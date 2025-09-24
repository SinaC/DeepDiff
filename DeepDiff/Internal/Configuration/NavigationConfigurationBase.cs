using System;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal abstract class NavigationConfigurationBase
    {
        public PropertyInfoExt NavigationProperty { get; } = null!;
        public Type NavigationChildType { get; } = null!;

        public NavigationConfigurationBase(Type entityType, PropertyInfo navigationProperty, Type navigationChildType)
        {
            NavigationProperty = new PropertyInfoExt(entityType, navigationProperty);
            NavigationChildType = navigationChildType;
        }
    }
}
