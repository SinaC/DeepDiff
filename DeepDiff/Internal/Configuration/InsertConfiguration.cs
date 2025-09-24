using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class InsertConfiguration
    {
        public IList<SetValueConfiguration> SetValueConfigurations { get; private set; } = new List<SetValueConfiguration>();

        public SetValueConfiguration AddSetValueConfiguration(Type entityType, PropertyInfo destinationProperty, object? value)
        {
            var config = new SetValueConfiguration(entityType, destinationProperty, value);
            SetValueConfigurations.Add(config);
            return config;
        }
    }
}
