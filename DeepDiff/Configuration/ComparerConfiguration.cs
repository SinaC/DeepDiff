using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class ComparerConfiguration
    {
        public Dictionary<Type, object> TypeSpecificNonGenericComparers { get; set; } = new Dictionary<Type, object>(); // IEqualityComparer<T> stored as object
        public Dictionary<Type, object> TypeSpecificGenericComparers { get; set; } = new Dictionary<Type, object>(); // IEqualityComparer<T> stored as object
        public Dictionary<PropertyInfo, object> PropertySpecificNonGenericComparers { get; set; } = new Dictionary<PropertyInfo, object>(); // IEqualityComparer<T> stored as object
        public Dictionary<PropertyInfo, object> PropertySpecificGenericComparers { get; set; } = new Dictionary<PropertyInfo, object>(); // IEqualityComparer<T> stored as object
    }
}
