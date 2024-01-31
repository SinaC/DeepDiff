using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class DeleteConfiguration
    {
        public SetValueConfiguration SetValueConfiguration { get; private set; } = null!;
        public bool GenerateOperations { get; private set; } = true;

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

        public void SetGenerationOperations(bool generationOperations)
        {
            GenerateOperations = generationOperations;
        }
    }
}
