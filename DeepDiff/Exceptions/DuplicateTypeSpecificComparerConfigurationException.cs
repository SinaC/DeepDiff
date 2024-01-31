using System;

namespace DeepDiff.Exceptions
{
    public class DuplicateTypeSpecificComparerConfigurationException : EntityConfigurationException
    {
        public DuplicateTypeSpecificComparerConfigurationException(Type entityType, Type propertyType)
            : base($"WithConverted<{propertyType}> has already been configured for {entityType}", entityType)
        {
        }
    }
}
