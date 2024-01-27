using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal abstract class NavigationConfigurationBase
    {
        public PropertyInfo NavigationProperty { get; set; } = null!;
        public Type NavigationChildType { get; set; } = null!;
        public IList<NavigationKeyConfiguration> NavigationKeyConfigurations { get; set; } = new List<NavigationKeyConfiguration>();

        public NavigationKeyConfiguration AddNavigationKeyConfiguration(PropertyInfo childNavigationKeyProperty, PropertyInfo navigationKeyProperty)
        {
            var navigationKeyConfiguration = new NavigationKeyConfiguration
            {
                ChildNavigationKeyProperty = childNavigationKeyProperty,
                NavigationKeyProperty = navigationKeyProperty
            };
            NavigationKeyConfigurations.Add(navigationKeyConfiguration);
            return navigationKeyConfiguration;
        }
    }
}
