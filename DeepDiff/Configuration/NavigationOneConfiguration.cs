using System;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class NavigationOneConfiguration: INavigationOneConfiguration
    {
        public PropertyInfo NavigationOneProperty { get; set; } = null!;
        public Type NavigationOneChildType { get; set; } = null!;
    }
}