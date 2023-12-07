using System;

namespace DeepDiff.Exceptions
{
    public sealed class MissingOnUpdateConfigurationException : MissingOperationConfigurationException
    {
        public MissingOnUpdateConfigurationException(Type entityType)
            : base(entityType, "OnUpdate")
        {
        }
    }
}
