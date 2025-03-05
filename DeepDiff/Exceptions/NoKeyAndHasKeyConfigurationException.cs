using System;

namespace DeepDiff.Exceptions
{
    public class NoKeyAndHasKeyConfigurationException : EntityConfigurationException
    {
        public NoKeyAndHasKeyConfigurationException(Type entityType)
            : base($"HasKey and NoKey both configured for {entityType}", entityType)
        {
        }
    }
}
