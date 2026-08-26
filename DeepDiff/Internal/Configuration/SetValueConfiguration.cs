using DeepDiff.Internal.Comparers;
using System.Reflection;

namespace DeepDiff.Internal.Configuration;

internal sealed class SetValueConfiguration(Type entityType, PropertyInfo destinationProperty, object? value)
{
    public PropertyInfoExt DestinationProperty { get; } = new PropertyInfoExt(entityType, destinationProperty);
    public object? Value { get; } = value;
}
