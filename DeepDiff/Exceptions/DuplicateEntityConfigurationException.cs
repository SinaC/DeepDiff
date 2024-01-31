using System;

namespace DeepDiff.Exceptions
{
    public sealed class DuplicateEntityConfigurationException : EntityConfigurationException
    {
        public DuplicateEntityConfigurationException(Type entityType)
            : base($"A configuration for {entityType} has already been defined", entityType)
        {
        }
    }
}
