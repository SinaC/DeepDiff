using System.Reflection;

namespace EntityMerger.Configuration;

internal sealed class NavigationOneConfiguration
{
    public PropertyInfo NavigationOneProperty { get; set; } = null!;
    public Type NavigationOneChildType { get; set; } = null!;
}
