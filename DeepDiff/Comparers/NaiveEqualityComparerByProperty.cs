using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Comparers
{
    internal sealed class NaiveEqualityComparerByProperty<T> : IEqualityComparer
        where T : class
    {
        private Type TypeOfT { get; }
        private IReadOnlyCollection<PropertyInfo> Properties { get; }
        private IReadOnlyDictionary<Type, IEqualityComparer> TypeSpecificComparers { get; }

        public NaiveEqualityComparerByProperty(IEnumerable<PropertyInfo> properties)
            : this(properties, null)
        {
        }

        public NaiveEqualityComparerByProperty(IEnumerable<PropertyInfo> properties, IReadOnlyDictionary<Type, IEqualityComparer> typeSpecificComparers)
        {
            TypeOfT = typeof(T);
            Properties = properties?.ToArray();
            TypeSpecificComparers = typeSpecificComparers;
        }

        public new bool Equals(object? left, object? right)
        {
            if (object.ReferenceEquals(left, right)) // will handle left == right == null
                return true;
            if (Properties == null)
                return left == null || left.Equals(right);
            if (left is not T)
                return false;
            if (right is not T)
                return false;
            if (TypeSpecificComparers?.TryGetValue(TypeOfT, out var typeSpecificComparer) == true)
                return typeSpecificComparer.Equals(left, right);
            foreach (var propertyInfo in Properties)
            {
                var existingValue = propertyInfo.GetValue(left);
                var newValue = propertyInfo.GetValue(right);

                if (TypeSpecificComparers?.TryGetValue(propertyInfo.PropertyType, out var propertyTypeSpecificComparer) == true)
                {
                    if (!propertyTypeSpecificComparer.Equals(existingValue, newValue))
                        return false;
                }
                else if (propertyInfo.PropertyType.IsValueType)
                {
                    if (!object.Equals(existingValue, newValue))
                        return false;
                }
                else
                {
                    if (!object.ReferenceEquals(existingValue, newValue) && (existingValue == null || !existingValue.Equals(newValue)))
                        return false;
                }
            }
            return true;
        }

        public int GetHashCode(object obj)
        {
            if (obj is not T)
                return obj.GetHashCode();
            if (Properties == null)
                return obj.GetHashCode();
            var hashCode = new HashCode();
            foreach (var propertyInfo in Properties)
            {
                var existingValue = propertyInfo.GetValue(obj);
                hashCode.Add(existingValue);
            }
            return hashCode.ToHashCode();
        }
    }
}