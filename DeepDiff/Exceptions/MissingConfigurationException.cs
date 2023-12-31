using System;

namespace DeepDiff.Exceptions
{
    public sealed class MissingConfigurationException : Exception
    {
        public Type EntityType { get; }

        public MissingConfigurationException(Type entityType)
            : base($"No configuration found for type {entityType}")
        {
            EntityType = entityType;
        }
    }
}
