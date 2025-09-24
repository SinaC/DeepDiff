using System;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class NavigationOneConfiguration : NavigationConfigurationBase
    {
        public NavigationOneConfiguration(Type entityType, PropertyInfo navigationProperty, Type navigationChildType)
            : base(entityType, navigationProperty, navigationChildType)
        {
        }
    }
}