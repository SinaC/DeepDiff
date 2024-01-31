using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class EntityConfiguration
    {
        public Type EntityType { get; }

        public KeyConfiguration KeyConfiguration { get; private set; } = null!;
        public ValuesConfiguration ValuesConfiguration { get; private set; } = null!;
        public IList<NavigationManyConfiguration> NavigationManyConfigurations { get; private set; } = new List<NavigationManyConfiguration>();
        public IList<NavigationOneConfiguration> NavigationOneConfigurations { get; private set; } = new List<NavigationOneConfiguration>();
        public UpdateConfiguration UpdateConfiguration { get; private set; } = null!;
        public InsertConfiguration InsertConfiguration { get; private set; } = null!;
        public DeleteConfiguration DeleteConfiguration { get; private set; } = null!;
        public ComparerConfiguration ComparerConfiguration { get; private set; } = null!;

        internal EntityConfiguration(Type entityType)
        {
            EntityType = entityType;
        }

        public KeyConfiguration SetKey(IEnumerable<PropertyInfo> keyProperties)
        {
            KeyConfiguration = new KeyConfiguration
            {
                KeyProperties = keyProperties.ToArray(),
            };
            return KeyConfiguration;
        }

        public ValuesConfiguration SetValues(IEnumerable<PropertyInfo> valuesProperties)
        {
            ValuesConfiguration = new ValuesConfiguration
            {
                ValuesProperties = valuesProperties.ToArray(),
            };
            return ValuesConfiguration;
        }

        public NavigationManyConfiguration AddNavigationMany(PropertyInfo navigationManyProperty, Type navigationManyDestinationType)
        {
            var navigationManyConfiguration = new NavigationManyConfiguration
            {
                NavigationProperty = navigationManyProperty,
                NavigationChildType = navigationManyDestinationType
            };
            NavigationManyConfigurations.Add(navigationManyConfiguration);
            return navigationManyConfiguration;
        }

        public NavigationOneConfiguration AddNavigationOne(PropertyInfo navigationOneProperty, Type navigationOneChildType)
        {
            var navigationOneConfiguration = new NavigationOneConfiguration
            {
                NavigationProperty = navigationOneProperty,
                NavigationChildType = navigationOneChildType
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

        public ComparerConfiguration GetOrSetWithComparer()
        {
            ComparerConfiguration ??= new ComparerConfiguration();
            return ComparerConfiguration;
        }

        public void CreateComparers()
        {
            KeyConfiguration.CreateComparers(EntityType, ComparerConfiguration);
            ValuesConfiguration?.CreateComparers(EntityType, ComparerConfiguration);
        }
    }
}