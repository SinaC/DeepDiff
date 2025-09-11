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

        public IReadOnlyCollection<PropertyInfo> KeyProperties { get; } = null!;

        public KeyConfiguration(IEnumerable<PropertyInfo> keyProperties)
        {
            KeyProperties = keyProperties.ToArray();
        }

        public void CreateComparers(Type typeOfT, ComparerConfiguration comparerConfiguration)
        {
            var naiveEqualityComparerByPropertyTypeOfT = typeof(NaiveEqualityComparerByProperty<>).MakeGenericType(typeOfT);
            NaiveEqualityComparer = (IComparerByProperty)Activator.CreateInstance(naiveEqualityComparerByPropertyTypeOfT, KeyProperties, comparerConfiguration?.TypeSpecificComparers, comparerConfiguration?.PropertySpecificComparers)!;

            var precompiledEqualityComparerByPropertyTypeOfT = typeof(PrecompiledEqualityComparerByProperty<>).MakeGenericType(typeOfT);
            PrecompiledEqualityComparer = (IComparerByProperty)Activator.CreateInstance(precompiledEqualityComparerByPropertyTypeOfT, KeyProperties, comparerConfiguration?.TypeSpecificComparers, comparerConfiguration?.PropertySpecificComparers)!;
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