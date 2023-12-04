using System;

namespace DeepDiff.Exceptions
{
    public class MissingOnInsertConfigurationException : MissingOperationConfigurationException
    {
        public MissingOnInsertConfigurationException(Type entityType)
            : base( entityType, "OnInsert")
        {
        }
    }
}
