using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class NavigationKeyConfiguration
    {
        public PropertyInfo ChildNavigationKeyProperty { get; } = null!;
        public PropertyInfo NavigationKeyProperty { get; } = null!;

        public NavigationKeyConfiguration(PropertyInfo childNavigationKeyProperty, PropertyInfo navigationKeyProperty)
        {
            ChildNavigationKeyProperty = childNavigationKeyProperty;
            NavigationKeyProperty = navigationKeyProperty;
        }
    }
}
