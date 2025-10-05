using DeepDiff.Internal.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class CopyValuesConfiguration
    {
        public IReadOnlyCollection<PropertyInfoExt> CopyValuesProperties { get; } = null!;

        public CopyValuesConfiguration(Type entityType, IEnumerable<PropertyInfo> copyValuesProperties)
        {
            CopyValuesProperties = copyValuesProperties.Select(x => new PropertyInfoExt(entityType, x)).ToArray();
        }
    }
}