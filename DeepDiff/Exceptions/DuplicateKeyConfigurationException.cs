using System;

namespace DeepDiff.Exceptions
{
    public class DuplicateKeyConfigurationException : EntityConfigurationException
    {
        public DuplicateKeyConfigurationException(Type entityType)
            : base($"HasKey has already been configured for {entityType}", entityType)
        {
        }
    }
}
