using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class UpdateConfiguration
    {
        public IList<SetValueConfiguration> SetValueConfigurations { get; private set; } = new List<SetValueConfiguration>();
        public CopyValuesConfiguration CopyValuesConfiguration { get; private set; } = null!;

        public SetValueConfiguration AddSetValueConfiguration(Type entityType, PropertyInfo destinationProperty, object? value)
        {
            var config = new SetValueConfiguration(entityType, destinationProperty, value);
            SetValueConfigurations.Add(config);
            return config;
        }

        public CopyValuesConfiguration SetCopyValuesConfiguration(Type entityType, IEnumerable<PropertyInfo> copyValuesProperties)
        {
            var config = new CopyValuesConfiguration(entityType, copyValuesProperties.ToArray());
            CopyValuesConfiguration = config;
            return config;
        }
    }
}
