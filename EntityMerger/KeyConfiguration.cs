using System.Collections;
using System.Reflection;

namespace EntityMerger.EntityMerger;

internal class KeyConfiguration : IKeyConfiguration
{
    public IReadOnlyCollection<PropertyInfo> KeyProperties { get; set; } = null!;
    public IEqualityComparer EqualityComparer { get; set; } = null!;

    public bool UsePrecompileEqualityComparer { get; set; } = true;

    public void DisablePrecompiledEqualityComparer()
    {
        UsePrecompileEqualityComparer = false;
    }
}
