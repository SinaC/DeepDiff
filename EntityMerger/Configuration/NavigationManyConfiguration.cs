using System.Reflection;

namespace EntityMerger.Configuration;

internal class NavigationManyConfiguration : INavigationManyConfiguration
{
    public PropertyInfo NavigationManyProperty { get; set; } = null!;
    public bool UseHashtable { get; private set; } = true;
    public Type NavigationManyChildType { get; set; } = null!;

    public INavigationManyConfiguration DisableHashtable()
    {
        UseHashtable = false;
        return this;
    }
}
