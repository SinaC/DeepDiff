using System;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class ForceUpdateIfEqualsConfiguration
    {
        public PropertyInfoExt CompareToProperty { get; } = null!;
        public object? CompareToValue { get; } = null!;

        public ForceUpdateIfEqualsConfiguration(Type entityType, PropertyInfo compareToProperty, object? compareToValue)
        {
            CompareToProperty = new PropertyInfoExt(entityType, compareToProperty);
            CompareToValue = compareToValue;
        }
    }
}
