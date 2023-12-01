using System;

namespace DeepDiff.Exceptions
{
    public abstract class DiffEntityConfigurationException : Exception
    {
        public Type EntityType { get; }

        public DiffEntityConfigurationException(string message, Type entityType)
            : base(message)
        {
            EntityType = entityType;
        }
    }
}
