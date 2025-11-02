using DeepDiff.Internal.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class KeyConfiguration
    {
        public IComparerByProperty EqualityComparer { get; private set; } = null!;

        public IReadOnlyCollection<PropertyInfoExt> KeyProperties { get; } = null!;

        public KeyConfiguration(Type entityType, IEnumerable<PropertyInfo> keyProperties)
        {
            KeyProperties = keyProperties.Select(x => new PropertyInfoExt(entityType, x)).ToArray();
        }

        public void CreateComparers(Type entityType, ComparerConfiguration comparerConfiguration)
        {
            // PrecompiledEqualityComparerByProperty<T> ctor
            // public PrecompiledEqualityComparerByProperty(IReadOnlyCollection<PropertyInfo> properties, IReadOnlyDictionary<Type, object>? typeSpecificComparers, IReadOnlyDictionary<PropertyInfo, object>? propertySpecificComparers) // object is in fact an IEqualityComparer<TProperty>
            var precompiledEqualityComparerByPropertyTypeOfT = typeof(PrecompiledEqualityComparerByProperty<>).MakeGenericType(entityType);
            EqualityComparer = (IComparerByProperty)Activator.CreateInstance(precompiledEqualityComparerByPropertyTypeOfT, KeyProperties.Select(x => x.PropertyInfo).ToArray(), comparerConfiguration?.TypeSpecificComparers, comparerConfiguration?.PropertySpecificComparers)!;
        }
    }
}