using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Internal.Comparers
{
    internal sealed class PrecompiledEqualityComparerByProperty<T> : IComparerByProperty
        where T : class
    {
        private EqualsFunc<T> EqualsFunc { get; init; }
        private Func<T, int> HasherFunc { get; init; }
        private CompareFunc<T> CompareFunc { get; init; }

        public PrecompiledEqualityComparerByProperty(IReadOnlyCollection<PropertyInfo> properties)
            : this(properties, null, null)
        {
        }

        public PrecompiledEqualityComparerByProperty(IReadOnlyCollection<PropertyInfo> properties, IReadOnlyDictionary<Type, object>? typeSpecificComparers, IReadOnlyDictionary<PropertyInfo, object>? propertySpecificComparers) // object is in fact an IEqualityComparer<TProperty>
        {
            EqualsFunc = ComparerExpressionGenerator.GenerateEqualsFunc<T>(properties, typeSpecificComparers, propertySpecificComparers);
            HasherFunc = ComparerExpressionGenerator.GenerateHasherFunc<T>(properties);
            CompareFunc = ComparerExpressionGenerator.GenerateCompareFunc<T>(properties, typeSpecificComparers, propertySpecificComparers);
        }

        public new bool Equals(object? left, object? right)
            => ReferenceEquals(left, right) // will handle left == right == null
                || left is T leftAsT && right is T rightAsT && EqualsFunc(leftAsT, rightAsT);

        public int GetHashCode(object obj)
            => obj is T objAsT ? HasherFunc(objAsT) : obj.GetHashCode();

        public CompareByPropertyResult Compare(object? left, object? right)
        {
            if (ReferenceEquals(left, right)) // will handle left == right == null
                return new CompareByPropertyResult(true);
            if (left is not T leftAsT)
                return new CompareByPropertyResult(false);
            if (right is not T rightAsT)
                return new CompareByPropertyResult(false);
            return CompareFunc(leftAsT, rightAsT);
        }
    }
}