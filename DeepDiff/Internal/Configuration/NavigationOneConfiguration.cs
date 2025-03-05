using System;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class NavigationOneConfiguration : NavigationConfigurationBase
    {
        public NavigationOneConfiguration(PropertyInfo navigationProperty, Type navigationChildType)
            : base(navigationProperty, navigationChildType)
        {
        }
    }
}