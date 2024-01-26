using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class DiffEntityConfiguration
    {
        public Type EntityType { get; }
        public IReadOnlyDictionary<Type, IEqualityComparer> TypeSpecificComparers { get; } = null!;

        public KeyConfiguration KeyConfiguration { get; private set; } = null!;
        public ValuesConfiguration ValuesConfiguration { get; private set; } = null!;
        public IList<NavigationManyConfiguration> NavigationManyConfigurations { get; private set; } = new List<NavigationManyConfiguration>();
        public IList<NavigationOneConfiguration> NavigationOneConfigurations { get; private set; } = new List<NavigationOneConfiguration>();
        public UpdateConfiguration UpdateConfiguration { get; private set; } = null!;
        public InsertConfiguration InsertConfiguration { get; private set; } = null!;
        public DeleteConfiguration DeleteConfiguration { get; private set; } = null!;

        internal DiffEntityConfiguration(Type entityType)
            : this(entityType, null)
        {
        }

        public DiffEntityConfiguration(Type entityType, IReadOnlyDictionary<Type, IEqualityComparer> typeSpecificComparers)
        {
            EntityType = entityType;
            TypeSpecificComparers = typeSpecificComparers;
        }

        public KeyConfiguration SetKey(IEnumerable<PropertyInfo> keyProperties, IEqualityComparer precompiledEqualityComparer, IEqualityComparer naiveEqualityComparer)
        {
            KeyConfiguration = new KeyConfiguration
            {
                KeyProperties = keyProperties.ToArray(),
                PrecompiledEqualityComparer = precompiledEqualityComparer,
                NaiveEqualityComparer = naiveEqualityComparer
            };
            return KeyConfiguration;
        }

        public ValuesConfiguration SetValues(IEnumerable<PropertyInfo> valuesProperties, IEqualityComparer precompiledEqualityComparer, IEqualityComparer naiveEqualityComparer)
        {
            ValuesConfiguration = new ValuesConfiguration
            {
                ValuesProperties = valuesProperties.ToArray(),
                PrecompiledEqualityComparer = precompiledEqualityComparer,
                NaiveEqualityComparer = naiveEqualityComparer
            };
            return ValuesConfiguration;
        }

        public NavigationManyConfiguration AddNavigationMany(PropertyInfo navigationManyProperty, Type navigationManyDestinationType)
        {
            var navigationManyConfiguration = new NavigationManyConfiguration
            {
                NavigationManyProperty = navigationManyProperty,
                NavigationManyChildType = navigationManyDestinationType
            };
            NavigationManyConfigurations.Add(navigationManyConfiguration);
            return navigationManyConfiguration;
        }

        public NavigationOneConfiguration AddNavigationOne(PropertyInfo navigationOneProperty, Type navigationOneChildType)
        {
            var navigationOneConfiguration = new NavigationOneConfiguration
            {
                NavigationOneProperty = navigationOneProperty,
                NavigationOneChildType = navigationOneChildType
            };
            NavigationOneConfigurations.Add(navigationOneConfiguration);
            return navigationOneConfiguration;
        }

        public UpdateConfiguration GetOrSetOnUpdate()
        {
            UpdateConfiguration ??= new UpdateConfiguration();
            return UpdateConfiguration;
        }

        public InsertConfiguration GetOrSetOnInsert()
        {
            InsertConfiguration ??= new InsertConfiguration();
            return InsertConfiguration;
        }

        public DeleteConfiguration GetOrSetOnDelete()
        {
            DeleteConfiguration ??= new DeleteConfiguration();
            return DeleteConfiguration;
        }
    }
}