using System;

namespace DeepDiff.Exceptions
{
    public sealed class MissingSetValueConfigurationException : EntityConfigurationException
    {
        public MissingSetValueConfigurationException(Type entityType, string configurationType)
            : base($"Missing SetValue configuration on {configurationType} for type {entityType}", entityType)
        {
        }
    }
}
