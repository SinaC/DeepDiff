using System;

namespace DeepDiff.Exceptions
{
    public class NoKeyAndKeyConfigurationException : EntityConfigurationException
    {
        public NoKeyAndKeyConfigurationException(Type entityType)
            : base($"HasKey and NoKey both configured for {entityType}", entityType)
        {
        }
    }
}
