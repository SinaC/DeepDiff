using DeepDiff.Comparers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class ValuesConfiguration : IValuesConfiguration
    {
        public IReadOnlyCollection<PropertyInfo> ValuesProperties { get; set; } = null!;
        public IEqualityComparer PrecompiledEqualityComparer { get; set; } = null!;
        public IEqualityComparer NaiveEqualityComparer { get; set; } = null!;

        public bool UsePrecompiledEqualityComparer { get; set; } = true;

        public void DisablePrecompiledEqualityComparer()
        {
            UsePrecompiledEqualityComparer = false;
        }

        public void CreateComparers(Type typeOfT, IReadOnlyDictionary<Type, IEqualityComparer> typeSpecificComparers, IReadOnlyDictionary<PropertyInfo, IEqualityComparer> propertySpecificComparers)
        {
            var naiveEqualityComparerByPropertyTypeOfT = typeof(NaiveEqualityComparerByProperty<>).MakeGenericType(typeOfT);
            NaiveEqualityComparer = (IEqualityComparer)Activator.CreateInstance(naiveEqualityComparerByPropertyTypeOfT, ValuesProperties, typeSpecificComparers); // TODO: use propertySpecificComparers

            var precompiledEqualityComparerByPropertyTypeOfT = typeof(PrecompiledEqualityComparerByProperty<>).MakeGenericType(typeOfT);
            PrecompiledEqualityComparer = (IEqualityComparer)Activator.CreateInstance(precompiledEqualityComparerByPropertyTypeOfT, ValuesProperties, typeSpecificComparers); // TODO: use propertySpecificComparers
        }
    }
}