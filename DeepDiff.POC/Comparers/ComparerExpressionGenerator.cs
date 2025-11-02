using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DeepDiff.POC.Comparers
{
    // most code has been ripped from https://blogs.u2u.be/peter/post/implementing-value-object-s-gethashcode
    internal delegate bool EqualsFunc<T>(T left, T right);
    internal delegate CompareByPropertyResult CompareFunc<T>(T left, T right);

    internal static class ComparerExpressionGenerator
    {
        internal static MethodInfo AddHashCodeMethod { get; }
        internal static MethodInfo ToHashCodeMethod { get; }
        internal static MethodInfo AddListOfCompareByPropertyResultDetailCodeMethod { get; set; }

        static ComparerExpressionGenerator()
        {
            // HashCode
            var hashCodeType = typeof(HashCode);
            AddHashCodeMethod = hashCodeType.GetMethods().Single(method => method.Name == nameof(HashCode.Add) && method.GetParameters().Length == 1);
            ToHashCodeMethod = hashCodeType.GetMethod(nameof(HashCode.ToHashCode), BindingFlags.Public | BindingFlags.Instance)!;
            // List<CompareByPropertyResultDetail>
            var listOfCompareByPropertyResultDetail = typeof(List<CompareByPropertyResultDetail>);
            AddListOfCompareByPropertyResultDetailCodeMethod = listOfCompareByPropertyResultDetail.GetMethod(nameof(List<CompareByPropertyResultDetail>.Add), BindingFlags.Public | BindingFlags.Instance)!;
        }

        // Equals
        internal static EqualsFunc<T> GenerateEqualsFunc<T>(IReadOnlyCollection<PropertyInfo> propertInfos, IReadOnlyDictionary<Type, object>? typeSpecificComparers, IReadOnlyDictionary<PropertyInfo, object>? propertySpecificComparers)
        {
            // generate the equivalent of
            // var equals = true;
            // foreach(var propertyInfo in propertyInfos)
            // {
            //      equals &= Equals(propertyInfo.GetValue(left), propertyInfo.GetValue(right));
            //             // --> Equals is propertySpecificComparers[propertyInfo.Type] if any, or typeSpecificComparers[propertyInfo.Type] if any, or IEquatable<propertyInfo.Type> if implemented or use left[propertyInfo].Equals(right[propertyInfo])
            //      if (!equals)
            //          return false;
            // }
            var left = Expression.Parameter(typeof(T), "left");
            var right = Expression.Parameter(typeof(T), "right");

            var equals = new List<Expression>();
            foreach (var propertyInfo in propertInfos)
            {
                var leftValue = Expression.Property(left, propertyInfo);
                var rightValue = Expression.Property(right, propertyInfo);
                var equalityExpression = GenerateEqualityExpression(leftValue, rightValue, propertyInfo, typeSpecificComparers, propertySpecificComparers);
                equals.Add(equalityExpression);
            }
            var ands = equals.Aggregate((left, right) => Expression.AndAlso(left, right));
            var andEquals = Expression.Lambda<EqualsFunc<T>>(ands, left, right).Compile();
            return andEquals;
        }

        // Compare
        internal static CompareFunc<T> GenerateCompareFunc<T>(IReadOnlyCollection<PropertyInfo> propertyInfos, IReadOnlyDictionary<Type, object>? typeSpecificComparers, IReadOnlyDictionary<PropertyInfo, object>? propertySpecificComparers)
        {
            var compareByPropertyResultDetailType = typeof(CompareByPropertyResultDetail);

            var left = Expression.Parameter(typeof(T), "left");
            var right = Expression.Parameter(typeof(T), "right");
            var resultVariable = Expression.Variable(typeof(CompareByPropertyResult), "result");
            var detailsParam = Expression.Variable(typeof(List<CompareByPropertyResultDetail>), "details");

            var compareByPropertyResultDetailCtor = compareByPropertyResultDetailType.GetConstructors().Single();

            var statements = new List<Expression>();
            // generate details = new List<CompareByPropertyResultDetail>()
            var createDetails = Expression.Assign(detailsParam, Expression.New(typeof(List<CompareByPropertyResultDetail>)));
            statements.Add(createDetails);
            foreach (var propertyInfo in propertyInfos)
            {
                var leftValue = Expression.Property(left, propertyInfo);
                var rightValue = Expression.Property(right, propertyInfo);

                // generate equality expression equivalent to left.{propertyInfo}.Equals(right.{propertyInfo})
                var equalExpression = GenerateEqualityExpression(leftValue, rightValue, propertyInfo, typeSpecificComparers, propertySpecificComparers);
                // generate detail = new CompareByPropertyResultDetail() { PropertyInfo = propertyInfo, OldValue = left.{propertyInfo}, NewValue = right.{propertyInfo} }
                var propertyInfoProperty = compareByPropertyResultDetailType.GetProperty(nameof(CompareByPropertyResultDetail.PropertyInfo))!;
                var propertyInfoAssignment = Expression.Bind(propertyInfoProperty, Expression.Constant(propertyInfo));

                var oldValueProperty = compareByPropertyResultDetailType.GetProperty(nameof(CompareByPropertyResultDetail.OldValue))!;
                var oldValueAssignment = Expression.Bind(oldValueProperty, Expression.Convert(leftValue, typeof(object)));

                var newValueProperty = compareByPropertyResultDetailType.GetProperty(nameof(CompareByPropertyResultDetail.NewValue))!;
                var newValueAssignment = Expression.Bind(newValueProperty, Expression.Convert(rightValue, typeof(object)));

                var newDetail = Expression.New(compareByPropertyResultDetailCtor);
                var memberInit = Expression.MemberInit(newDetail, propertyInfoAssignment, oldValueAssignment, newValueAssignment);

                // generate add detail to details
                var addToListExpression = Expression.Call(detailsParam, AddListOfCompareByPropertyResultDetailCodeMethod, memberInit);

                // generate if !isEqual then add to details
                var ifThen = Expression.IfThen(
                    Expression.Not(equalExpression),
                    addToListExpression);

                statements.Add(ifThen);
            }
            // generate result = new CompareByPropertyResult(details);
            var createResult = Expression.Assign(resultVariable, Expression.New(typeof(CompareByPropertyResult).GetConstructors().Single(x => x.GetParameters().First().ParameterType == typeof(IReadOnlyCollection<CompareByPropertyResultDetail>)), detailsParam));
            statements.Add(createResult);

            var block = Expression.Block
            (
                typeof(CompareByPropertyResult),
                new[] { detailsParam, resultVariable },
                statements
            );

            var convertFunc = Expression.Lambda<CompareFunc<T>>(block, left, right).Compile();
            return convertFunc;
        }

        // Hash  (when properties count is 1, this code is slower than the naive implementation)
        internal static Func<T, int> GenerateHasherFunc<T>(IReadOnlyCollection<PropertyInfo> propertyInfos)
        {
            if (propertyInfos.Count == 1)
            {
                // Generates the equivalent of
                // return this.Property.GetHashCode();
                var propertyInfo = propertyInfos.First();
                var propertyGetHashCodeMethod = propertyInfo.PropertyType.GetMethods().Single(x => x.Name == nameof(object.GetHashCode) && x.GetParameters().Length == 0);
                var obj = Expression.Parameter(typeof(T), "obj");
                var callGetHashCode = Expression.Call(Expression.Property(obj, propertyInfo), propertyGetHashCodeMethod);
                var hasher = Expression.Lambda<Func<T, int>>(callGetHashCode, obj).Compile();
                return hasher;
            }
            else
            {
                // Generates the equivalent of
                // var hash = new HashCode();
                // hash.Add(this.Price);
                // hash.Add(this.When);
                // return hash.ToHashCode();
                var obj = Expression.Parameter(typeof(T), "obj");
                var hashCode = Expression.Variable(typeof(HashCode), "hashCode");

                var parts = GenerateAddToHashCodeExpressions(obj, hashCode, propertyInfos);
                parts.Insert(0, Expression.Assign(hashCode, Expression.New(typeof(HashCode))));
                parts.Add(Expression.Call(hashCode, ToHashCodeMethod));
                var body = parts.ToArray();

                var block = Expression.Block(
                    typeof(int),
                    new ParameterExpression[] { hashCode },
                    body);

                var hasher = Expression.Lambda<Func<T, int>>(block, obj).Compile();
                return hasher;
            }
        }

        private static List<Expression> GenerateAddToHashCodeExpressions(ParameterExpression obj, ParameterExpression hashCode, IReadOnlyCollection<PropertyInfo> propertyInfos)
        {
            var adders = new List<Expression>();
            foreach (var propertyInfo in propertyInfos)
            {
                var boundAddMethod = AddHashCodeMethod.MakeGenericMethod(propertyInfo.PropertyType);
                adders.Add(Expression.Call(hashCode, boundAddMethod, Expression.Property(obj, propertyInfo)));
            }
            return adders;
        }

        // Equals by property
        private static Expression GenerateEqualityExpression(MemberExpression leftValue, MemberExpression rightValue, PropertyInfo propertyInfo, IReadOnlyDictionary<Type, object>? typeSpecificComparers, IReadOnlyDictionary<PropertyInfo, object>? propertySpecificComparers)
        {
            var propertyType = propertyInfo.PropertyType;

            // if property specific comparer
            //      if comparer is IEqualityComparer<>
            //          use comparer.Equals(left, right)
            //      else
            //          use comparer.Equals((object)left, (object)right)
            // else if type specific comparer
            //      if comparer is IEqualityComparer<>
            //          use comparer.Equals(left, right)
            //      else
            //          use comparer.Equals((object)left, (object)right)
            // else
            //      if type implements IEquatable<>
            //          use left.Equals(right)
            //      else
            //          use left.Equals((object)right)
            MethodInfo equalMethod;
            Expression equalCall;
            if (propertySpecificComparers?.TryGetValue(propertyInfo, out var propertySpecificComparer) == true)
            {
                var equalityComparerType = typeof(IEqualityComparer<>).MakeGenericType(propertyType);
                if (equalityComparerType.IsAssignableFrom(propertySpecificComparer.GetType()))
                {
                    equalMethod = propertySpecificComparer.GetType().GetMethod(nameof(Equals), new Type[] { propertyType, propertyType })!;
                    equalCall = Expression.Call(Expression.Constant(propertySpecificComparer), equalMethod, leftValue, rightValue);
                }
                else
                {
                    equalMethod = propertySpecificComparer.GetType().GetMethod(nameof(Equals), new Type[] { typeof(object), typeof(object) })!;
                    equalCall = Expression.Call(Expression.Constant(propertySpecificComparer), equalMethod, Expression.Convert(leftValue, typeof(object)), Expression.Convert(rightValue, typeof(object)));
                }
            }
            else if (typeSpecificComparers?.TryGetValue(propertyType, out var propertyTypeSpecificComparer) == true) // generates Comparer.Equals((object)left, (object)right)
            {
                var equalityComparerType = typeof(IEqualityComparer<>).MakeGenericType(propertyType);
                if (equalityComparerType.IsAssignableFrom(propertyTypeSpecificComparer.GetType()))
                {
                    equalMethod = propertyTypeSpecificComparer.GetType().GetMethod(nameof(Equals), new Type[] { propertyType, propertyType })!;
                    equalCall = Expression.Call(Expression.Constant(propertyTypeSpecificComparer), equalMethod, leftValue, rightValue);
                }
                else
                {
                    equalMethod = propertyTypeSpecificComparer.GetType().GetMethod(nameof(Equals), new Type[] { typeof(object), typeof(object) })!;
                    equalCall = Expression.Call(Expression.Constant(propertyTypeSpecificComparer), equalMethod, Expression.Convert(leftValue, typeof(object)), Expression.Convert(rightValue, typeof(object)));
                }
            }
            else
            {
                var equitableType = typeof(IEquatable<>).MakeGenericType(propertyType);
                if (equitableType.IsAssignableFrom(propertyType)) // generates left.Equals(right)
                {
                    equalMethod = equitableType.GetMethod(nameof(Equals), new Type[] { propertyType })!;
                    equalCall = Expression.Call(leftValue, equalMethod, rightValue);
                }
                else // generates left.Equals((object)right)
                {
                    equalMethod = propertyType.GetMethod(nameof(Equals), new Type[] { typeof(object) })!;
                    equalCall = Expression.Call(leftValue, equalMethod, Expression.Convert(rightValue, typeof(object)));
                }
            }

            // if value type, call directly Equals
            if (propertyInfo.PropertyType.IsValueType)
            {
                // property is value type, no need to check for null, so directly call Equals
                return equalCall;
            }
            // if not value type, check null then call Equals
            else
            {
                // generate
                //       Expression<Func<T, T, bool>> ce = (T x, T y) => object.ReferenceEquals(x, y) || (x != null && x.Equals(y));
                var refEqual = Expression.ReferenceEqual(leftValue, rightValue);
                var nullConst = Expression.Constant(null);
                var leftIsNotNull = Expression.Not(Expression.ReferenceEqual(leftValue, nullConst));
                var leftIsNotNullAndIsEqual = Expression.AndAlso(leftIsNotNull, equalCall);
                var either = Expression.OrElse(refEqual, leftIsNotNullAndIsEqual);

                return either;
            }
        }
    }
}
