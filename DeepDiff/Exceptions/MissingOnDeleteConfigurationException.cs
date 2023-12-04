using System;

namespace DeepDiff.Exceptions
{
    public class MissingOnDeleteConfigurationException : MissingOperationConfigurationException
    {
        public MissingOnDeleteConfigurationException(Type entityType)
            : base( entityType, "OnDelete")
        {
        }
    }
}
