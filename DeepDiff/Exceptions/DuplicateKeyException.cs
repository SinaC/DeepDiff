using System;

namespace DeepDiff.Exceptions
{
    public class DuplicateKeyException : Exception
    {
        public Type EntityType { get; }
        public string Keys { get; }

        public DuplicateKeyException(Type entityType, string keys) : base($"Duplicate key found on type {entityType} with key {keys}")
        {
            EntityType = entityType;
            Keys = keys;
        }
    }
}
