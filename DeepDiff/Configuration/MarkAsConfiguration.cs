using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class MarkAsConfiguration
    {
        public PropertyInfo DestinationProperty { get; set; } = null!;
        public object Value { get; set; } = null!;
    }
}