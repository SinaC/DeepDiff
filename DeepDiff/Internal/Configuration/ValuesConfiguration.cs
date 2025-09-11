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

        public IReadOnlyCollection<PropertyInfo> ValuesProperties { get; } = null!;

        public ValuesConfiguration(IEnumerable<PropertyInfo> valuesProperties)
        {
            ValuesProperties = valuesProperties.ToArray();
        }

        public void CreateComparers(Type typeOfT, ComparerConfiguration comparerConfiguration)
        {
            var naiveEqualityComparerByPropertyTypeOfT = typeof(NaiveEqualityComparerByProperty<>).MakeGenericType(typeOfT);
            NaiveEqualityComparer = (IComparerByProperty)Activator.CreateInstance(naiveEqualityComparerByPropertyTypeOfT, ValuesProperties, comparerConfiguration?.TypeSpecificComparers, comparerConfiguration?.PropertySpecificComparers)!;

            var precompiledEqualityComparerByPropertyTypeOfT = typeof(PrecompiledEqualityComparerByProperty<>).MakeGenericType(typeOfT);
            PrecompiledEqualityComparer = (IComparerByProperty)Activator.CreateInstance(precompiledEqualityComparerByPropertyTypeOfT, ValuesProperties, comparerConfiguration?.TypeSpecificComparers, comparerConfiguration?.PropertySpecificComparers)!;
        }

        public IComparerByProperty GetComparer(EqualityComparers equalityComparers)
            => equalityComparers switch
            {
                EqualityComparers.Naive=> NaiveEqualityComparer,
                EqualityComparers.Precompiled => PrecompiledEqualityComparer,
                _ => throw new ArgumentOutOfRangeException(nameof(equalityComparers))
            };
    }
}