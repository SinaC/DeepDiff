using System;

namespace DeepDiff.Exceptions
{
    public class DuplicateValuesConfigurationException : DiffEntityConfigurationException
    {
        public DuplicateValuesConfigurationException(Type entityType)
            : base($"HasValues has already been configured for {entityType}", entityType)
        {
        }
    }
}
