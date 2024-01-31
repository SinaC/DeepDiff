using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class SetValueConfiguration
    {
        public PropertyInfo DestinationProperty { get; init; } = null!;
        public object Value { get; init; } = null!;
    }
}
