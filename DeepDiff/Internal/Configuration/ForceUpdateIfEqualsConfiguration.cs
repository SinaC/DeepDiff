using DeepDiff.Internal.Comparers;
using System.Reflection;

namespace DeepDiff.Internal.Configuration;

internal sealed class ForceUpdateIfEqualsConfiguration(Type entityType, PropertyInfo compareToProperty, object? compareToValue)
{
    public PropertyInfoExt CompareToProperty { get; } = new PropertyInfoExt(entityType, compareToProperty);
    public object? CompareToValue { get; } = compareToValue;
}
