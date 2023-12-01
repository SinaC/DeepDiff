using System;

namespace DeepDiff.Exceptions
{
    public class DuplicateDiffEntityConfigurationException : DiffEntityConfigurationException
    {
        public DuplicateDiffEntityConfigurationException(Type entityType)
            : base($"A configuration for {entityType} has already been defined", entityType)
        {
        }
    }
}
