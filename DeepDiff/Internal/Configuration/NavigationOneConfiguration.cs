using System.Reflection;

namespace DeepDiff.Internal.Configuration;

internal sealed class NavigationOneConfiguration(Type entityType, PropertyInfo navigationProperty, Type navigationChildType) : NavigationConfigurationBase(entityType, navigationProperty, navigationChildType)
{
}