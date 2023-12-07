using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class DeleteConfiguration
    {
        public SetValueConfiguration SetValueConfiguration { get; set; } = null!;

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
    }
}
