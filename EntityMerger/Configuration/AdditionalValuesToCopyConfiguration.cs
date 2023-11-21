using System.Reflection;

namespace EntityMerger.Configuration;

internal sealed class AdditionalValuesToCopyConfiguration
{
    public IReadOnlyCollection<PropertyInfo> AdditionalValuesToCopyProperties { get; set; } = null!;
}
