using System.Reflection;

namespace EntityMerger.Configuration;

internal sealed class MarkAsConfiguration
{
    public PropertyInfo DestinationProperty { get; set; } = null!;
    public object Value { get; set; } = null!;
}
