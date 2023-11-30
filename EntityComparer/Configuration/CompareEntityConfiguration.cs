using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntityComparer.Configuration
{
    internal sealed class CompareEntityConfiguration
    {
        public Type EntityType { get; }
        public KeyConfiguration KeyConfiguration { get; private set; } = null!;
        public ValuesConfiguration ValuesConfiguration { get; private set; } = null!;
        public AdditionalValuesToCopyConfiguration AdditionalValuesToCopyConfiguration { get; private set; } = null!;
        public IList<NavigationManyConfiguration> NavigationManyConfigurations { get; private set; } = new List<NavigationManyConfiguration>();
        public IList<NavigationOneConfiguration> NavigationOneConfigurations { get; private set; } = new List<NavigationOneConfiguration>();
        public IDictionary<CompareEntityOperation, MarkAsConfiguration> MarkAsByOperation { get; private set; } = new Dictionary<CompareEntityOperation, MarkAsConfiguration>();

        public CompareEntityConfiguration(Type entityType)
        {
            EntityType = entityType;
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

        public AdditionalValuesToCopyConfiguration SetAdditionalValuesToCopy(IEnumerable<PropertyInfo> additionalValuesToCopyProperties)
        {
            AdditionalValuesToCopyConfiguration = new AdditionalValuesToCopyConfiguration
            {
                AdditionalValuesToCopyProperties = additionalValuesToCopyProperties.ToArray(),
            };
            return AdditionalValuesToCopyConfiguration;
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

        public MarkAsConfiguration SetMarkAsInserted(PropertyInfo destinationProperty, object value)
        {
            var markAsInsertedConfiguration = SetMarkAs(destinationProperty, value, CompareEntityOperation.Insert);
            return markAsInsertedConfiguration;
        }

        public MarkAsConfiguration SetMarkAsUpdated(PropertyInfo destinationProperty, object value)
        {
            var markAsUpdatedConfiguration = SetMarkAs(destinationProperty, value, CompareEntityOperation.Update);
            return markAsUpdatedConfiguration;
        }

        public MarkAsConfiguration SetMarkAsDeleted(PropertyInfo destinationProperty, object value)
        {
            var markAsDeletedConfiguration = SetMarkAs(destinationProperty, value, CompareEntityOperation.Delete);
            return markAsDeletedConfiguration;
        }

        private MarkAsConfiguration SetMarkAs(PropertyInfo destinationProperty, object value, CompareEntityOperation operation)
        {
            var markAsConfiguration = new MarkAsConfiguration
            {
                DestinationProperty = destinationProperty,
                Value = value
            };
            MarkAsByOperation[operation] = markAsConfiguration;
            return markAsConfiguration;
        }
    }
}