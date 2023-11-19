using System.Reflection;

namespace EntityMerger.EntityMerger;

internal class MarkAsConfiguration
{
    public PropertyInfo DestinationProperty { get; set; } = null!;
    public object Value { get; set; } = null!;
}
