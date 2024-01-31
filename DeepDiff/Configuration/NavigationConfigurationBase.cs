using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal abstract class NavigationConfigurationBase
    {
        public PropertyInfo NavigationProperty { get; } = null!;
        public Type NavigationChildType { get; } = null!;

        public IList<NavigationKeyConfiguration> NavigationKeyConfigurations { get; private set; } = new List<NavigationKeyConfiguration>();

        public NavigationConfigurationBase(PropertyInfo navigationProperty, Type navigationChildType)
        {
            NavigationProperty = navigationProperty;
            NavigationChildType = navigationChildType;
        }

        public NavigationKeyConfiguration AddNavigationKeyConfiguration(PropertyInfo childNavigationKeyProperty, PropertyInfo navigationKeyProperty)
        {
            var navigationKeyConfiguration = new NavigationKeyConfiguration(childNavigationKeyProperty, navigationKeyProperty);
            NavigationKeyConfigurations.Add(navigationKeyConfiguration);
            return navigationKeyConfiguration;
        }
    }
}
