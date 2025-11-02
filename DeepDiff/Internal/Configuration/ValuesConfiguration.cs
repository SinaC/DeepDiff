using DeepDiff.Internal.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class ValuesConfiguration
    {
        public IComparerByProperty EqualityComparer { get; private set; } = null!;

        public IReadOnlyCollection<PropertyInfoExt> ValuesProperties { get; } = null!;

        public ValuesConfiguration(Type entityType, IEnumerable<PropertyInfo> valuesProperties)
        {
            ValuesProperties = valuesProperties.Select(x => new PropertyInfoExt(entityType, x)).ToArray();
        }

        public void CreateComparers(Type typeOfT, ComparerConfiguration comparerConfiguration)
        {
            // PrecompiledEqualityComparerByProperty<T> ctor
            // public PrecompiledEqualityComparerByProperty(IEnumerable<PropertyInfo> properties, IReadOnlyDictionary<Type, object>? typeSpecificComparers, IReadOnlyDictionary<PropertyInfo, object>? propertySpecificComparers) // object is in fact an IEqualityComparer<TProperty>
            var precompiledEqualityComparerByPropertyTypeOfT = typeof(PrecompiledEqualityComparerByProperty<>).MakeGenericType(typeOfT);
            EqualityComparer = (IComparerByProperty)Activator.CreateInstance(precompiledEqualityComparerByPropertyTypeOfT, ValuesProperties.Select(x => x.PropertyInfo).ToArray(), comparerConfiguration?.TypeSpecificComparers, comparerConfiguration?.PropertySpecificComparers)!;
        }
    }
}