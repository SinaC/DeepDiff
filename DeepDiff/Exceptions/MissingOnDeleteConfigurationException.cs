using System;

namespace DeepDiff.Exceptions
{
    public sealed class MissingOnDeleteConfigurationException : MissingOperationConfigurationException
    {
        public MissingOnDeleteConfigurationException(Type entityType)
            : base( entityType, "OnDelete")
        {
        }
    }
}
