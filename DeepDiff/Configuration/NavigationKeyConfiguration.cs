using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class NavigationKeyConfiguration
    {
        public PropertyInfo ChildNavigationKeyProperty { get; set; }
        public PropertyInfo NavigationKeyProperty { get; set; }
    }
}
