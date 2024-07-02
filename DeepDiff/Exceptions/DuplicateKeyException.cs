using System;
using System.Collections.Generic;

namespace DeepDiff.Exceptions
{
    public class DuplicateKeyException : Exception
    {
        public Type EntityType { get; }
        public IReadOnlyDictionary<string, string> Keys { get; }

        public DuplicateKeyException(Type entityType, IReadOnlyDictionary<string, string> keys) : base($"Duplicate key found on type {entityType}")
        {
            EntityType = entityType;
            Keys = keys;
        }
    }
}
