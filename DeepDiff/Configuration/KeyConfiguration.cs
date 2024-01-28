using DeepDiff.Comparers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class KeyConfiguration : IKeyConfiguration
    {
        public IReadOnlyCollection<PropertyInfo> KeyProperties { get; set; } = null!;
        public IEqualityComparer PrecompiledEqualityComparer { get; set; } = null!;
        public IEqualityComparer NaiveEqualityComparer { get; set; } = null!;

        public bool UsePrecompiledEqualityComparer { get; set; } = true;

        public void DisablePrecompiledEqualityComparer()
        {
            UsePrecompiledEqualityComparer = false;
        }

        public void CreateComparers(Type typeOfT, IReadOnlyDictionary<Type, IEqualityComparer> typeSpecificComparers)
        {
            var naiveEqualityComparerByPropertyTypeOfT = typeof(NaiveEqualityComparerByProperty<>).MakeGenericType(typeOfT);
            NaiveEqualityComparer = (IEqualityComparer)Activator.CreateInstance(naiveEqualityComparerByPropertyTypeOfT, KeyProperties, typeSpecificComparers);

            var precompiledEqualityComparerByPropertyTypeOfT = typeof(PrecompiledEqualityComparerByProperty<>).MakeGenericType(typeOfT);
            PrecompiledEqualityComparer = (IEqualityComparer)Activator.CreateInstance(precompiledEqualityComparerByPropertyTypeOfT, KeyProperties, typeSpecificComparers);
        }
    }
}