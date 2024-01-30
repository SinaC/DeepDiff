using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Comparers
{
    internal sealed class NaiveEqualityComparerByProperty<T> : IComparerByProperty
        where T : class
    {
        private IReadOnlyCollection<PropertyInfo> Properties { get; }
        private IReadOnlyDictionary<Type, IEqualityComparer> TypeSpecificComparers { get; }
        private IReadOnlyDictionary<PropertyInfo, IEqualityComparer> PropertySpecificComparers { get; }

        public NaiveEqualityComparerByProperty(IEnumerable<PropertyInfo> properties)
            : this(properties, null, null)
        {
        }

        public NaiveEqualityComparerByProperty(IEnumerable<PropertyInfo> properties, IReadOnlyDictionary<Type, IEqualityComparer> typeSpecificComparers, IReadOnlyDictionary<PropertyInfo, IEqualityComparer> propertySpecificComparers)
        {
            Properties = properties?.ToArray();
            TypeSpecificComparers = typeSpecificComparers;
            PropertySpecificComparers = propertySpecificComparers;
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
            foreach (var propertyInfo in Properties)
            {
                var oldValue = propertyInfo.GetValue(left);
                var newValue = propertyInfo.GetValue(right);

                if (PropertySpecificComparers?.TryGetValue(propertyInfo, out var propertySpecificComparer) == true)
                {
                    if (!propertySpecificComparer.Equals(oldValue, newValue))
                        return false;
                }
                else if (TypeSpecificComparers?.TryGetValue(propertyInfo.PropertyType, out var propertyTypeSpecificComparer) == true)
                {
                    if (!propertyTypeSpecificComparer.Equals(oldValue, newValue))
                        return false;
                }
                else if (propertyInfo.PropertyType.IsValueType)
                {
                    if (!object.Equals(oldValue, newValue))
                        return false;
                }
                else
                {
                    if (!object.ReferenceEquals(oldValue, newValue) && (oldValue == null || !oldValue.Equals(newValue)))
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

        public CompareByPropertyResult Compare(object? left, object? right)
        {
            if (object.ReferenceEquals(left, right)) // will handle left == right == null
                return new CompareByPropertyResult(true);
            if (Properties == null)
                return new CompareByPropertyResult(left == null || left.Equals(right));
            if (left is not T)
                return new CompareByPropertyResult(false);
            if (right is not T)
                return new CompareByPropertyResult(false);
            var details = new List<CompareByPropertyResultDetail>();
            foreach (var propertyInfo in Properties)
            {
                var isEqualByProperty = true; // equal by default
                var oldValue = propertyInfo.GetValue(left);
                var newValue = propertyInfo.GetValue(right);

                if (PropertySpecificComparers?.TryGetValue(propertyInfo, out var propertySpecificComparer) == true)
                {
                    if (!propertySpecificComparer.Equals(oldValue, newValue))
                        isEqualByProperty = false;
                }
                else if (TypeSpecificComparers?.TryGetValue(propertyInfo.PropertyType, out var propertyTypeSpecificComparer) == true)
                {
                    if (!propertyTypeSpecificComparer.Equals(oldValue, newValue))
                        isEqualByProperty = false;
                }
                else if (propertyInfo.PropertyType.IsValueType)
                {
                    if (!object.Equals(oldValue, newValue))
                        isEqualByProperty = false;
                }
                else
                {
                    if (!object.ReferenceEquals(oldValue, newValue) && (oldValue == null || !oldValue.Equals(newValue)))
                        isEqualByProperty = false;
                }
                if (!isEqualByProperty)
                    //details.Add(new CompareByPropertyResultDetail(propertyInfo,oldValue,newValue));
                    details.Add(new CompareByPropertyResultDetail { PropertyInfo = propertyInfo, OldValue = oldValue, NewValue = newValue });
            }
            return new CompareByPropertyResult(details);
        }
    }
}