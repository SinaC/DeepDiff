using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class ComparerConfiguration
    {
        public Dictionary<Type, IEqualityComparer> TypeSpecificComparers { get; set; } = new Dictionary<Type, IEqualityComparer>();
        public Dictionary<PropertyInfo, IEqualityComparer> PropertySpecificComparers { get; set; } = new Dictionary<PropertyInfo, IEqualityComparer>();
    }
}
