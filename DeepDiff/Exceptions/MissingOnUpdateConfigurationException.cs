using System;

namespace DeepDiff.Exceptions
{
    public class MissingOnUpdateConfigurationException : MissingOperationConfigurationException
    {
        public MissingOnUpdateConfigurationException(Type entityType)
            : base(entityType, "OnUpdate")
        {
        }
    }
}
