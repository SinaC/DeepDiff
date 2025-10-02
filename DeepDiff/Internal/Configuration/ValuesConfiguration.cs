using DeepDiff.Configuration;
using DeepDiff.Internal.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class ValuesConfiguration
    {
        private IComparerByProperty PrecompiledEqualityComparer { get; set; } = null!;
        private IComparerByProperty NaiveEqualityComparer { get; set; } = null!;

        public IReadOnlyCollection<PropertyInfoExt> ValuesProperties { get; } = null!;

        public ValuesConfiguration(Type entityType, IEnumerable<PropertyInfo> valuesProperties)
        {
            ValuesProperties = valuesProperties.Select(x => new PropertyInfoExt(entityType, x)).ToArray();
        }

        public void CreateComparers(Type typeOfT, ComparerConfiguration comparerConfiguration)
        {
            // NaiveEqualityComparerByProperty<T> ctor
            // public NaiveEqualityComparerByProperty(IEnumerable<PropertyInfoExt> propertyExts, IReadOnlyDictionary<Type, object>? typeSpecificComparers, IReadOnlyDictionary<PropertyInfo, object>? propertySpecificComparers) // object is in fact an IEqualityComparer<TProperty>
            var naiveEqualityComparerByPropertyTypeOfT = typeof(NaiveEqualityComparerByProperty<>).MakeGenericType(typeOfT);
            NaiveEqualityComparer = (IComparerByProperty)Activator.CreateInstance(naiveEqualityComparerByPropertyTypeOfT, ValuesProperties, comparerConfiguration?.TypeSpecificComparers, comparerConfiguration?.PropertySpecificComparers)!;

            // PrecompiledEqualityComparerByProperty<T> ctor
            // public PrecompiledEqualityComparerByProperty(IEnumerable<PropertyInfo> properties, IReadOnlyDictionary<Type, object>? typeSpecificComparers, IReadOnlyDictionary<PropertyInfo, object>? propertySpecificComparers) // object is in fact an IEqualityComparer<TProperty>
            var precompiledEqualityComparerByPropertyTypeOfT = typeof(PrecompiledEqualityComparerByProperty<>).MakeGenericType(typeOfT);
            PrecompiledEqualityComparer = (IComparerByProperty)Activator.CreateInstance(precompiledEqualityComparerByPropertyTypeOfT, ValuesProperties.Select(x => x.PropertyInfo).ToArray(), comparerConfiguration?.TypeSpecificComparers, comparerConfiguration?.PropertySpecificComparers)!;
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