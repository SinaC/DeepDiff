using System.Reflection;

namespace DeepDiff.Configuration
{
    internal class SetValueConfiguration
    {
        public PropertyInfo DestinationProperty { get; set; } = null!;
        public object Value { get; set; } = null!;
    }
}
