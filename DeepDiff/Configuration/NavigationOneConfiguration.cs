using System.Reflection;
using System;

namespace DeepDiff.Configuration
{
    internal sealed class NavigationOneConfiguration : NavigationConfigurationBase
    {
        public NavigationOneConfiguration(PropertyInfo navigationProperty, Type navigationChildType)
            : base(navigationProperty, navigationChildType)
        {
        }
    }
}