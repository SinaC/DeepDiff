using System;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class NavigationManyConfiguration : INavigationManyConfiguration
    {
        public PropertyInfo NavigationManyProperty { get; set; } = null!;
        public Type NavigationManyChildType { get; set; } = null!;
    }
}