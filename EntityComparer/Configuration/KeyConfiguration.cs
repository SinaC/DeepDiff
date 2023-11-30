using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace EntityComparer.Configuration
{
    internal sealed class KeyConfiguration : IKeyConfiguration
    {
        public IReadOnlyCollection<PropertyInfo> KeyProperties { get; set; } = null!;
        public IEqualityComparer PrecompiledEqualityComparer { get; set; } = null!;
        public IEqualityComparer NaiveEqualityComparer { get; set; } = null!;

        public bool UsePrecompiledEqualityComparer { get; set; } = true;

        public void DisablePrecompiledEqualityComparer()
        {
            UsePrecompiledEqualityComparer = false;
        }
    }
}