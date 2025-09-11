using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class DeleteConfiguration
    {
        public IList<SetValueConfiguration> SetValueConfigurations { get; } = new List<SetValueConfiguration>();

        public SetValueConfiguration AddSetValueConfiguration(PropertyInfo destinationProperty, object? value)
        {
            var config = new SetValueConfiguration(destinationProperty, value);
            SetValueConfigurations.Add(config);
            return config;
        }
    }
}
