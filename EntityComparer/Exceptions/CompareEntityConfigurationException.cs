using System;

namespace EntityComparer.Exceptions
{
    public abstract class CompareEntityConfigurationException : Exception
    {
        public Type EntityType { get; }

        public CompareEntityConfigurationException(string message, Type entityType)
            : base(message)
        {
            EntityType = entityType;
        }
    }
}
