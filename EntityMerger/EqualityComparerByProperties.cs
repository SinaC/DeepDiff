using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityMerger;

public sealed class EqualityComparerByProperties<T> : IEqualityComparer
    where T : class
{
    private CompareFunc<T> Comparer { get; init; }
    private Func<T, int> Hasher { get; init; }

    public EqualityComparerByProperties(IEnumerable<PropertyInfo> propertyInfos)
    {
        Comparer = ExpressionGenerater.GenerateComparer<T>(propertyInfos);
        Hasher = ExpressionGenerater.GenerateHasher<T>(propertyInfos);
    }

    public new bool Equals(object? left, object? right)
        => object.ReferenceEquals(left, right)
      || (left is T leftAsT && right is T rightAsT && this.Comparer(leftAsT, rightAsT));

    public int GetHashCode(object obj)
        => obj is T objAsT ? this.Hasher(objAsT) : obj.GetHashCode();
}

// most code has been ripped from https://blogs.u2u.be/peter/post/implementing-value-object-s-gethashcode
internal delegate bool CompareFunc<T>(T left, T right);

/// <summary>
/// Helper class to take care of generating the comparer expression using "Just once" reflection.
/// </summary>
internal static class ExpressionGenerater
{
    internal static MethodInfo SequenceEqualMethod { get; }
    internal static MethodInfo SequenceHashCodeMethod { get; }
    internal static MethodInfo AddHashCodeMethod { get; }
    internal static MethodInfo ToHashCodeMethod { get; }
    internal static MethodInfo AddCollectionHashCodeMethod { get; }

    static ExpressionGenerater()
    {
        SequenceEqualMethod = typeof(Enumerable)
         .GetMethods(bindingAttr: BindingFlags.Public | BindingFlags.Static)
         .Single(methodInfo => methodInfo.Name == nameof(Enumerable.SequenceEqual) && methodInfo.GetParameters().Length == 2);

        Type hashCodeType = typeof(HashCode);
        AddHashCodeMethod = hashCodeType.GetMethods()
          .Single(method => method.Name == nameof(HashCode.Add) && method.GetParameters().Length == 1);
        AddCollectionHashCodeMethod = AddHashCodeMethod.MakeGenericMethod(typeof(int));
        ToHashCodeMethod =
          hashCodeType.GetMethod(nameof(HashCode.ToHashCode), BindingFlags.Public | BindingFlags.Instance)!;

        SequenceHashCodeMethod = typeof(ExpressionGenerater)
          .GetMethods(bindingAttr: BindingFlags.NonPublic | BindingFlags.Static)
          .Single(methodInfo => methodInfo.Name == nameof(ExpressionGenerater.AddHashCodeMembersForCollection));
    }

    private static Expression GenerateEqualityExpression(ParameterExpression left, ParameterExpression right, PropertyInfo propInfo)
    {
        Type propertyType = propInfo.PropertyType;
        Type equitableType = typeof(IEquatable<>).MakeGenericType(propertyType);

        MethodInfo equalMethod;
        Expression equalCall;
        if (equitableType.IsAssignableFrom(propertyType))
        {
            equalMethod = equitableType.GetMethod(nameof(Equals), new Type[] { propertyType });
            equalCall = Expression.Call(Expression.Property(left, propInfo), equalMethod, Expression.Property(right, propInfo));
        }
        else
        {
            equalMethod = propertyType.GetMethod(nameof(Equals), new Type[] { typeof(object) });
            equalCall = Expression.Call(Expression.Property(left, propInfo), equalMethod, Expression.Convert(Expression.Property(right, propInfo), typeof(object)));
        }

        if (propInfo.PropertyType.IsValueType)
        {
            // Property is value type, no need to check for null, so directly call Equals
            return equalCall;
        }
        else
        {
            // Generate
            //       Expression<Func<T, T, bool>> ce = (T x, T y) => object.ReferenceEquals(x, y) || (x != null && x.Equals(y));

            Expression leftValue = Expression.Property(left, propInfo);
            Expression rightValue = Expression.Property(right, propInfo);
            Expression refEqual = Expression.ReferenceEqual(leftValue, rightValue);
            Expression nullConst = Expression.Constant(null);
            Expression leftIsNotNull = Expression.Not(Expression.ReferenceEqual(leftValue, nullConst));
            Expression leftIsNotNullAndIsEqual = Expression.AndAlso(leftIsNotNull, equalCall);
            Expression either = Expression.OrElse(refEqual, leftIsNotNullAndIsEqual);

            return either;
        }
    }

    internal static CompareFunc<T> GenerateComparer<T>(IEnumerable<PropertyInfo> propertyInfos)
    {
        var comparers = new List<Expression>();
        ParameterExpression left = Expression.Parameter(typeof(T), "left");
        ParameterExpression right = Expression.Parameter(typeof(T), "right");

        foreach (PropertyInfo propInfo in propertyInfos)
        {
            comparers.Add(GenerateEqualityExpression(left, right, propInfo));
        }
        Expression ands = comparers.Aggregate((left, right) => Expression.AndAlso(left, right));
        CompareFunc<T>? andComparer = Expression.Lambda<CompareFunc<T>>(ands, left, right).Compile();
        return andComparer;
    }

    internal static int AddHashCodeMembersForCollection<T>(IEnumerable<T> coll)
    {
        var hashCode = new HashCode();
        if (coll != null)
        {
            foreach (T el in coll)
            {
                hashCode.Add(el != null ? el.GetHashCode() : 0);
            }
        }
        return hashCode.ToHashCode();
    }

    internal static Func<T, int> GenerateHasher<T>(IEnumerable<PropertyInfo> propertyInfos)
    {
        // Generates the equivalent of
        // var hash = new HashCode();
        // hash.Add(this.Price);
        // hash.Add(this.When);
        // AddHasCodeMembersForCollection(hash, this.Hobbies) where this.Hobbies has DeepCompare attribute.
        // return hash.ToHashCode();
        ParameterExpression obj = Expression.Parameter(typeof(T), "obj");
        ParameterExpression hashCode = Expression.Variable(typeof(HashCode), "hashCode");

        List<Expression> parts = GenerateAddToHashCodeExpressions(obj, hashCode, propertyInfos);
        parts.Insert(0, Expression.Assign(hashCode, Expression.New(typeof(HashCode))));
        parts.Add(Expression.Call(hashCode, ToHashCodeMethod));
        Expression[] body = parts.ToArray();

        BlockExpression block = Expression.Block(
          type: typeof(int),
          variables: new ParameterExpression[] { hashCode },
          expressions: body);

        Func<T, int> hasher = Expression.Lambda<Func<T, int>>(block, obj).Compile();
        return hasher;
    }

    private static List<Expression> GenerateAddToHashCodeExpressions(ParameterExpression obj, ParameterExpression hashCode, IEnumerable<PropertyInfo> propertyInfos)
    {
        var adders = new List<Expression>();
        foreach (PropertyInfo propInfo in propertyInfos)
        {
            MethodInfo boundAddMethod = AddHashCodeMethod.MakeGenericMethod(propInfo.PropertyType);
            adders.Add(Expression.Call(instance: hashCode, boundAddMethod, Expression.Property(obj, propInfo)));
        }
        return adders;
    }
}
