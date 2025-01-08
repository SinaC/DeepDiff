using System;

namespace DeepDiff.Exceptions
{
    public class AbstractEntityConfigurationException : Exception
    {
        public Type EntityType { get; }

        public AbstractEntityConfigurationException(Type entityType)
            : base($"Configuration found for abstract type {entityType}")
        {
            EntityType = entityType;
        }
    }
}
