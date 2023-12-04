using System;

namespace DeepDiff.Exceptions
{
    public abstract class MissingOperationConfigurationException : DiffEntityConfigurationException
    {
        public MissingOperationConfigurationException(Type entityType, string configurationType)
            : base($"No {configurationType} configuration has been configured for type {entityType}", entityType)
        {
        }
    }
}
