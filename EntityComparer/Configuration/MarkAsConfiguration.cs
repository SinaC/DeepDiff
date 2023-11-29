using System.Reflection;

namespace EntityComparer.Configuration;

internal sealed class MarkAsConfiguration
{
    public PropertyInfo DestinationProperty { get; set; } = null!;
    public object Value { get; set; } = null!;
}
