using System;

namespace DeepDiff.Exceptions
{
    public abstract class EntityConfigurationException : Exception
    {
        public Type EntityType { get; }

        public EntityConfigurationException(string message, Type entityType)
            : base(message)
        {
            EntityType = entityType;
        }
    }
}
