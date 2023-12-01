using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class ValuesConfiguration : IValuesConfiguration
    {
        public IReadOnlyCollection<PropertyInfo> ValuesProperties { get; set; } = null!;
        public IEqualityComparer PrecompiledEqualityComparer { get; set; } = null!;
        public IEqualityComparer NaiveEqualityComparer { get; set; } = null!;

        public bool UsePrecompiledEqualityComparer { get; set; } = true;

        public void DisablePrecompiledEqualityComparer()
        {
            UsePrecompiledEqualityComparer = false;
        }
    }
}