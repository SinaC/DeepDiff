using System.Collections;
using System.Reflection;

namespace EntityMerger.EntityMerger;

internal class CalculatedValueConfiguration: ICalculatedValueConfiguration
{
    public IReadOnlyCollection<PropertyInfo> CalculatedValueProperties { get; set; } = null!;
    public IEqualityComparer EqualityComparer { get; set; } = null!;

    public bool UsePrecompileEqualityComparer { get; set; } = true;

    public void DisablePrecompiledEqualityComparer()
    {
        UsePrecompileEqualityComparer = false;
    }
}
