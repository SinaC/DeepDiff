using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class NavigationKeyConfiguration
    {
        public PropertyInfo NavigationKeyProperty { get; set; }
        public PropertyInfo ChildNavigationKeyProperty { get; set; }
    }
}
