using DeepDiff.Internal.Comparers;
using System.Reflection;

namespace DeepDiff.Internal.Configuration;

internal sealed class CopyValuesConfiguration(Type entityType, IEnumerable<PropertyInfo> copyValuesProperties)
{
    public IReadOnlyCollection<PropertyInfoExt> CopyValuesProperties { get; } = copyValuesProperties.Select(x => new PropertyInfoExt(entityType, x)).ToArray();
}