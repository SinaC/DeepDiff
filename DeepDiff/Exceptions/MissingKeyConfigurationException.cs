using System;

namespace DeepDiff.Exceptions
{
    public class MissingKeyConfigurationException : DiffEntityConfigurationException
    {
        public MissingKeyConfigurationException(Type entityType)
            : base($"No Key configuration has been configured for type {entityType}", entityType)
        {
        }
    }
}
