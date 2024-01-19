using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class UpdateConfiguration
    {
        public SetValueConfiguration SetValueConfiguration { get; set; } = null!;
        public CopyValuesConfiguration CopyValuesConfiguration { get; set; } = null!;
        public bool GenerateOperations { get; set; } = false;

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

        public CopyValuesConfiguration SetCopyValuesConfiguration(IEnumerable<PropertyInfo> copyValuesProperties)
        {
            var config = new CopyValuesConfiguration
            {
                CopyValuesProperties = copyValuesProperties.ToArray()
            };
            CopyValuesConfiguration = config;
            return config;
        }

        public void SetGenerationOperations()
        {
            GenerateOperations = true;
        }
    }
}
