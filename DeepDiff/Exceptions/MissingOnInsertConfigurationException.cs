using System;

namespace DeepDiff.Exceptions
{
    public sealed class MissingOnInsertConfigurationException : MissingOperationConfigurationException
    {
        public MissingOnInsertConfigurationException(Type entityType)
            : base( entityType, "OnInsert")
        {
        }
    }
}
