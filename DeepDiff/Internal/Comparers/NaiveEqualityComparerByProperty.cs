using DeepDiff.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Internal.Comparers
{
    internal sealed class NaiveEqualityComparerByProperty<T> : IComparerByProperty
        where T : class
    {
        private IReadOnlyCollection<PropertyInfoExt> PropertyExts { get; }
        private IReadOnlyDictionary<Type, object>? TypeSpecificComparers { get; }
        private IReadOnlyDictionary<PropertyInfo, object>? PropertySpecificComparers { get; }

        public NaiveEqualityComparerByProperty(IReadOnlyCollection<PropertyInfoExt> propertyExts)
            : this(propertyExts, null, null)
        {
        }

        public NaiveEqualityComparerByProperty(IReadOnlyCollection<PropertyInfoExt> propertyExts, IReadOnlyDictionary<Type, object>? typeSpecificComparers, IReadOnlyDictionary<PropertyInfo, object>? propertySpecificComparers) // object is in fact an IEqualityComparer<TProperty>
        {
            PropertyExts = propertyExts;
            TypeSpecificComparers = typeSpecificComparers;
            PropertySpecificComparers = propertySpecificComparers;
        }

        public new bool Equals(object? left, object? right)
        {
            if (ReferenceEquals(left, right)) // will handle left == right == null
                return true;
            if (PropertyExts == null)
                return left == null || left.Equals(right);
            if (left is not T)
                return false;
            if (right is not T)
                return false;
            foreach (var propertyInfoExt in PropertyExts)
            {
                var oldValue = propertyInfoExt.GetValue(left)!;
                var newValue = propertyInfoExt.GetValue(right)!;

                if (PropertySpecificComparers?.TryGetValue(propertyInfoExt.PropertyInfo, out var propertySpecificComparer) == true)
                {
                    if (!PropertyEquals(propertyInfoExt.PropertyInfo, propertySpecificComparer, oldValue, newValue))
                        return false;
                }
                else if (TypeSpecificComparers?.TryGetValue(propertyInfoExt.PropertyType, out var propertyTypeSpecificComparer) == true)
                {
                    if (!PropertyEquals(propertyInfoExt.PropertyInfo, propertyTypeSpecificComparer, oldValue, newValue))
                        return false;
                }
                else if (propertyInfoExt.PropertyType.IsValueType)
                {
                    if (!object.Equals(oldValue, newValue))
                        return false;
                }
                else
                {
                    if (!ReferenceEquals(oldValue, newValue) && (oldValue == null || !oldValue.Equals(newValue)))
                        return false;
                }
            }
            return true;
        }

        public int GetHashCode(object obj)
        {
            if (obj is not T)
                return obj.GetHashCode();
            if (PropertyExts == null)
                return obj.GetHashCode();
            var hashCode = new HashCode();
            foreach (var propertyInfo in PropertyExts)
            {
                var existingValue = propertyInfo.GetValue(obj);
                hashCode.Add(existingValue);
            }
            return hashCode.ToHashCode();
        }

        public CompareByPropertyResult Compare(object? left, object? right)
        {
            if (ReferenceEquals(left, right)) // will handle left == right == null
                return new CompareByPropertyResult(true);
            if (PropertyExts == null)
                return new CompareByPropertyResult(left == null || left.Equals(right));
            if (left is not T)
                return new CompareByPropertyResult(false);
            if (right is not T)
                return new CompareByPropertyResult(false);
            var details = new List<CompareByPropertyResultDetail>();
            foreach (var propertyInfo in PropertyExts)
            {
                var isEqualByProperty = true; // equal by default
                var oldValue = propertyInfo.GetValue(left);
                var newValue = propertyInfo.GetValue(right);

                if (PropertySpecificComparers?.TryGetValue(propertyInfo.PropertyInfo, out var propertySpecificComparer) == true)
                {
                    if (!PropertyEquals(propertyInfo.PropertyInfo, propertySpecificComparer, oldValue, newValue))
                        isEqualByProperty = false;
                }
                else if (TypeSpecificComparers?.TryGetValue(propertyInfo.PropertyType, out var propertyTypeSpecificComparer) == true)
                {
                    if (!PropertyEquals(propertyInfo.PropertyInfo, propertyTypeSpecificComparer, oldValue, newValue))
                        isEqualByProperty = false;
                }
                else if (propertyInfo.PropertyType.IsValueType)
                {
                    if (!object.Equals(oldValue, newValue))
                        isEqualByProperty = false;
                }
                else
                {
                    if (!ReferenceEquals(oldValue, newValue) && (oldValue == null || !oldValue.Equals(newValue)))
                        isEqualByProperty = false;
                }
                if (!isEqualByProperty)
                    details.Add(new CompareByPropertyResultDetail { PropertyInfo = propertyInfo.PropertyInfo, OldValue = oldValue, NewValue = newValue });
            }
            return new CompareByPropertyResult(details);
        }

        private bool PropertyEquals(PropertyInfo propertyInfo, object equalityComparer, object? left, object? right)
        {
            var equalityComparerType = typeof(IEqualityComparer<>).MakeGenericType(propertyInfo.PropertyType);
            if (equalityComparerType.IsAssignableFrom(equalityComparer.GetType()))
            {
                var equalMethod = equalityComparerType.GetMethod(nameof(Equals), new[] { propertyInfo.PropertyType, propertyInfo.PropertyType })!;
                return (bool)equalMethod.Invoke(equalityComparer, new object[] { left!, right! })!;
            }
            throw new InvalidComparerForPropertyTypeException(propertyInfo.PropertyType); // should never been raised
        }
    }
}