using DeepDiff.Internal.Comparers;
using System.Reflection;

namespace DeepDiff.Internal.Configuration;

internal abstract class NavigationConfigurationBase(Type entityType, PropertyInfo navigationProperty, Type navigationChildType)
{
    public PropertyInfoExt NavigationProperty { get; } = new PropertyInfoExt(entityType, navigationProperty);
    public Type NavigationChildType { get; } = navigationChildType;
}
