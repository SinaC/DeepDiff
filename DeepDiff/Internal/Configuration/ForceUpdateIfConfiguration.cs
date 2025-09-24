using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class ForceUpdateIfConfiguration
    {
        public bool NestedEntitiesModifiedEnabled { get; private set; }
        public IList<ForceUpdateIfEqualsConfiguration> ForceUpdateIfEqualsConfigurations { get; private set; } = new List<ForceUpdateIfEqualsConfiguration>();

        public void EnableNestedEntitiesModified()
        {
            NestedEntitiesModifiedEnabled = true;
        }

        public ForceUpdateIfEqualsConfiguration AddEqualsConfiguration(Type entityType, PropertyInfo compareToProperty, object? compareToValue)
        {
            var config = new ForceUpdateIfEqualsConfiguration(entityType, compareToProperty, compareToValue);
            ForceUpdateIfEqualsConfigurations.Add(config);
            return config;
        }
    }
}
