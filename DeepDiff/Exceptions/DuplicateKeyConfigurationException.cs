using System;

namespace DeepDiff.Exceptions
{
    public class DuplicateKeyConfigurationException : DiffEntityConfigurationException
    {
        public DuplicateKeyConfigurationException(Type entityType)
            : base($"HasKey has already been configured for {entityType}", entityType)
        {
        }
    }
}
