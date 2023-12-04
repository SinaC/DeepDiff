using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal class UpdateConfiguration
    {
        public SetValueConfiguration SetValueConfiguration { get; set; } = null!;
        public CopyValuesConfiguration CopyValuesConfiguration { get; set;} = null!;

        public SetValueConfiguration SetSetValueConfiguration(PropertyInfo destinationProperty, object value)
        {
            var config = new SetValueConfiguration
            {
                DestinationProperty = destinationProperty,
                Value = value
            };
            SetValueConfiguration = config;
            return config;
        }

        public CopyValuesConfiguration SetCopyValuesConfiguration(IEnumerable<PropertyInfo> copyValuesConfigurationProperties)
        {
            var config = new CopyValuesConfiguration
            {
                CopyValuesConfigurationProperties = copyValuesConfigurationProperties.ToArray()
            };
            CopyValuesConfiguration = config;
            return config;
        }
    }
}
