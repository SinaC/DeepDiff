using System;

namespace DeepDiff.Exceptions
{
    public class NoKeyEntityInNavigationManyException : Exception
    {
        public Type EntityType { get; }

        public NoKeyEntityInNavigationManyException(Type entityType)
            : base($"NoKey set on type {entityType} but used in a HasMany configuration")
        {
            EntityType = entityType;
        }
    }
}
