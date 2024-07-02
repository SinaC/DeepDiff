using System;

namespace DeepDiff.Exceptions
{
    public class DuplicateKeysException : Exception
    {
        public Type EntityType { get; }
        public string Keys { get; }

        public DuplicateKeysException(Type entityType, string keys) : base($"Duplicate key(s) found on type {entityType} with key(s) {keys}")
        {
            EntityType = entityType;
            Keys = keys;
        }
    }
}
