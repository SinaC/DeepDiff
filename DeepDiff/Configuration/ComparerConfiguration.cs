using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeepDiff.Configuration
{
    internal sealed class ComparerConfiguration
    {
        public Dictionary<Type, object> TypeSpecificNonGenericComparers { get; } = new Dictionary<Type, object>(); // IEqualityComparer<T> stored as object
        public Dictionary<Type, object> TypeSpecificGenericComparers { get; } = new Dictionary<Type, object>(); // IEqualityComparer<T> stored as object
        public Dictionary<PropertyInfo, object> PropertySpecificNonGenericComparers { get; } = new Dictionary<PropertyInfo, object>(); // IEqualityComparer<T> stored as object
        public Dictionary<PropertyInfo, object> PropertySpecificGenericComparers { get; } = new Dictionary<PropertyInfo, object>(); // IEqualityComparer<T> stored as object
    }
}
