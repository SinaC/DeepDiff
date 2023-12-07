using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class SetValueConfiguration
    {
        public PropertyInfo DestinationProperty { get; set; } = null!;
        public object Value { get; set; } = null!;
    }
}
