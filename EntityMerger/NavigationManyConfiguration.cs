using System.Reflection;

namespace EntityMerger.EntityMerger;

internal class NavigationManyConfiguration
{
    public List<PropertyInfo> NavigationManyProperties { get; set; } = new List<PropertyInfo>();
}
