using System.Reflection;

namespace EntityMerger.Configuration;

internal class ValueToCopyConfiguration
{
    public IReadOnlyCollection<PropertyInfo> CopyValueProperties { get; set; } = null!;
}
