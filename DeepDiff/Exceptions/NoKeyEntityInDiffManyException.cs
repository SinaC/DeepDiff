using System;

namespace DeepDiff.Exceptions
{
    public class NoKeyEntityInDiffManyException : Exception
    {
        public Type EntityType { get; }

        public NoKeyEntityInDiffManyException(Type entityType)
            : base($"NoKey set on type {entityType} but used while performing DiffMany")
        {
            EntityType = entityType;
        }
    }
}
