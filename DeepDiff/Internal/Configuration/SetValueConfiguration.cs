using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class SetValueConfiguration
    {
        public PropertyInfo DestinationProperty { get; } = null!;
        public object? Value { get; }

        public SetValueConfiguration(PropertyInfo destinationProperty, object? value)
        {
            DestinationProperty = destinationProperty;
            Value = value;
        }
    }
}
