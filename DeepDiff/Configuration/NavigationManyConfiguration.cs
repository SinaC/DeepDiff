using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class NavigationManyConfiguration
    {
        public PropertyInfo NavigationManyProperty { get; set; } = null!;
        public Type NavigationManyChildType { get; set; } = null!;
        public IList<NavigationKeyConfiguration> NavigationKeyConfigurations { get; set; } = new List<NavigationKeyConfiguration>();

        public NavigationKeyConfiguration AddNavigationKeyConfiguration(PropertyInfo navigationKeyProperty, PropertyInfo childNavigationKeyProperty)
        {
            var navigationKeyConfiguration = new NavigationKeyConfiguration
            {
                NavigationKeyProperty = navigationKeyProperty,
                ChildNavigationKeyProperty = childNavigationKeyProperty
            };
            NavigationKeyConfigurations.Add(navigationKeyConfiguration);
            return navigationKeyConfiguration;
        }
    }
}