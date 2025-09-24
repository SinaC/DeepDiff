using System;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class SetValueConfiguration
    {
        public PropertyInfoExt DestinationProperty { get; } = null!;
        public object? Value { get; }

        public SetValueConfiguration(Type entityType, PropertyInfo destinationProperty, object? value)
        {
            DestinationProperty = new PropertyInfoExt(entityType, destinationProperty);
            Value = value;
        }
    }
}
