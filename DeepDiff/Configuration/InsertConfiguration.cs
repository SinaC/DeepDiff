using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class InsertConfiguration
    {
        public SetValueConfiguration SetValueConfiguration { get; set; } = null!;
        public bool GenerateOperations { get; set; } = true;

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

        public void SetGenerationOperations(bool generateOperations)
        {
            GenerateOperations = generateOperations;
        }
    }
}
