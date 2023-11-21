using System.Collections;
using System.Reflection;

namespace EntityMerger.Configuration;

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
