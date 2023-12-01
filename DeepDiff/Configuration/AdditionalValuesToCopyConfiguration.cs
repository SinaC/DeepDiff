using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class AdditionalValuesToCopyConfiguration
    {
        public IReadOnlyCollection<PropertyInfo> AdditionalValuesToCopyProperties { get; set; } = null!;
    }
}