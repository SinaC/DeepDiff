using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class UpdateConfiguration
    {
        public SetValueConfiguration SetValueConfiguration { get; private set; } = null!;
        public CopyValuesConfiguration CopyValuesConfiguration { get; private set; } = null!;
        public bool GenerateOperations { get; private set; } = true;

        public SetValueConfiguration SetSetValueConfiguration(PropertyInfo destinationProperty, object value)
        {
            var config = new SetValueConfiguration(destinationProperty, value);
            SetValueConfiguration = config;
            return config;
        }

        public CopyValuesConfiguration SetCopyValuesConfiguration(IEnumerable<PropertyInfo> copyValuesProperties)
        {
            var config = new CopyValuesConfiguration(copyValuesProperties.ToArray());
            CopyValuesConfiguration = config;
            return config;
        }

        public void SetGenerationOperations(bool generateOperations)
        {
            GenerateOperations = generateOperations;
        }
    }
}
