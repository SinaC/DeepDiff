using System.Collections;
using System.Reflection;

namespace EntityMerger.EntityMerger;

internal class CalculatedValueConfiguration: ICalculatedValueConfiguration
{
    public IReadOnlyCollection<PropertyInfo> CalculatedValueProperties { get; set; } = null!;
    public IEqualityComparer PrecompiledEqualityComparer { get; set; } = null!;
    public IEqualityComparer NaiveEqualityComparer { get; set; } = null!;

    public bool UsePrecompiledEqualityComparer { get; set; } = true;

    public void DisablePrecompiledEqualityComparer()
    {
        UsePrecompiledEqualityComparer = false;
    }
}
