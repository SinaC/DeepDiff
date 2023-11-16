using System.Reflection;

namespace EntityMerger.EntityMerger;

internal class NavigationOneConfiguration
{
    public List<PropertyInfo> NavigationOneProperties { get; set; } = new List<PropertyInfo>();
}
