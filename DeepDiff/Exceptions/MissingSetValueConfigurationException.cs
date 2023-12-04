using System;

namespace DeepDiff.Exceptions
{
    public class MissingSetValueConfigurationException : DiffEntityConfigurationException
    {
        public MissingSetValueConfigurationException(Type entityType, string configurationType)
            : base($"Missing SetValue configuration on {configurationType} for type {entityType}", entityType)
        {
        }
    }
}
