using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class ComparerConfiguration
    {
        public Dictionary<Type, IEqualityComparer> TypeSpecificNonGenericComparers { get; set; } = new Dictionary<Type, IEqualityComparer>(); // IEqualityComparer<T> converted to IEqualityComparer using NonGenericEqualityComparer
        public Dictionary<Type, object> TypeSpecificGenericComparers { get; set; } = new Dictionary<Type, object>(); // IEqualityComparer<T> stored as object
        public Dictionary<PropertyInfo, IEqualityComparer> PropertySpecificNonGenericComparers { get; set; } = new Dictionary<PropertyInfo, IEqualityComparer>(); // IEqualityComparer<T> converted to IEqualityComparer using NonGenericEqualityComparer
        public Dictionary<PropertyInfo, object> PropertySpecificGenericComparers { get; set; } = new Dictionary<PropertyInfo, object>(); // IEqualityComparer<T> stored as object
    }
}
