using System.Reflection;

namespace EntityComparer.Configuration;

internal sealed class AdditionalValuesToCopyConfiguration
{
    public IReadOnlyCollection<PropertyInfo> AdditionalValuesToCopyProperties { get; set; } = null!;
}
