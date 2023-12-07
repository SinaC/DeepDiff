using System;

namespace DeepDiff.Exceptions
{
    public sealed class EmptyConfigurationException : DiffEntityConfigurationException
    {
        public EmptyConfigurationException(Type entityType, string configurationType)
            : base($"{configurationType} configuration for type {entityType} is empty", entityType)
        {
        }
    }
}
