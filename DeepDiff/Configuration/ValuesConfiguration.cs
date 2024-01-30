using DeepDiff.Comparers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class ValuesConfiguration : IValuesConfiguration
    {
        public IReadOnlyCollection<PropertyInfo> ValuesProperties { get; set; } = null!;
        public IComparerByProperty PrecompiledEqualityComparer { get; set; } = null!;
        public IComparerByProperty NaiveEqualityComparer { get; set; } = null!;

        public bool UsePrecompiledEqualityComparer { get; set; } = true;

        public void DisablePrecompiledEqualityComparer()
        {
            UsePrecompiledEqualityComparer = false;
        }

        public void CreateComparers(Type typeOfT, ComparerConfiguration comparerConfiguration)
        {
            var naiveEqualityComparerByPropertyTypeOfT = typeof(NaiveEqualityComparerByProperty<>).MakeGenericType(typeOfT);
            NaiveEqualityComparer = (IComparerByProperty)Activator.CreateInstance(naiveEqualityComparerByPropertyTypeOfT, ValuesProperties, comparerConfiguration?.TypeSpecificNonGenericComparers, comparerConfiguration?.PropertySpecificNonGenericComparers);

            var precompiledEqualityComparerByPropertyTypeOfT = typeof(PrecompiledEqualityComparerByProperty<>).MakeGenericType(typeOfT);
            PrecompiledEqualityComparer = (IComparerByProperty)Activator.CreateInstance(precompiledEqualityComparerByPropertyTypeOfT, ValuesProperties, comparerConfiguration?.TypeSpecificGenericComparers, comparerConfiguration?.PropertySpecificGenericComparers);
        }
    }
}