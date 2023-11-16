using System.Reflection;

namespace EntityMerger.EntityMerger;

internal class MarkAsConfiguration
{
    public PropertyInfo DestinationProperty { get; set; }
    public object Value { get; set; }
}
