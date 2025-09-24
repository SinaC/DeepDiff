using DeepDiff.Configuration;
using DeepDiff.Internal.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class KeyConfiguration
    {
        private IComparerByProperty PrecompiledEqualityComparer { get; set; } = null!;
        private IComparerByProperty NaiveEqualityComparer { get; set; } = null!;

        public IReadOnlyCollection<PropertyInfoExt> KeyProperties { get; } = null!;

        public KeyConfiguration(Type entityType, IEnumerable<PropertyInfo> keyProperties)
        {
            KeyProperties = keyProperties.Select(x => new PropertyInfoExt(entityType, x)).ToArray();
        }

        public void CreateComparers(Type entityType, ComparerConfiguration comparerConfiguration)
        {
            // NaiveEqualityComparerByProperty<T> ctor
            // public NaiveEqualityComparerByProperty(IReadOnlyCollection<PropertyInfoExt> propertyExts, IReadOnlyDictionary<Type, object>? typeSpecificComparers, IReadOnlyDictionary<PropertyInfo, object>? propertySpecificComparers) // object is in fact an IEqualityComparer<TProperty>
            var naiveEqualityComparerByPropertyTypeOfT = typeof(NaiveEqualityComparerByProperty<>).MakeGenericType(entityType);
            NaiveEqualityComparer = (IComparerByProperty)Activator.CreateInstance(naiveEqualityComparerByPropertyTypeOfT, KeyProperties, comparerConfiguration?.TypeSpecificComparers, comparerConfiguration?.PropertySpecificComparers)!;

            // PrecompiledEqualityComparerByProperty<T> ctor
            // public PrecompiledEqualityComparerByProperty(IReadOnlyCollection<PropertyInfo> properties, IReadOnlyDictionary<Type, object>? typeSpecificComparers, IReadOnlyDictionary<PropertyInfo, object>? propertySpecificComparers) // object is in fact an IEqualityComparer<TProperty>
            var precompiledEqualityComparerByPropertyTypeOfT = typeof(PrecompiledEqualityComparerByProperty<>).MakeGenericType(entityType);
            PrecompiledEqualityComparer = (IComparerByProperty)Activator.CreateInstance(precompiledEqualityComparerByPropertyTypeOfT, KeyProperties.Select(x => x.PropertyInfo).ToArray(), comparerConfiguration?.TypeSpecificComparers, comparerConfiguration?.PropertySpecificComparers)!;
        }

        public IComparerByProperty GetComparer(EqualityComparers equalityComparers)
            => equalityComparers switch
            {
                EqualityComparers.Naive => NaiveEqualityComparer,
                EqualityComparers.Precompiled => PrecompiledEqualityComparer,
                _ => throw new ArgumentOutOfRangeException(nameof(equalityComparers))
            };
    }
}