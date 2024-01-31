using System;
using System.Reflection;

namespace DeepDiff.Exceptions
{
    public class DuplicatePropertySpecificComparerConfigurationException : EntityConfigurationException
    {
        public DuplicatePropertySpecificComparerConfigurationException(Type entityType, PropertyInfo propertyInfo)
            : base($"WithConverted(x => {propertyInfo.Name}) has already been configured for {entityType}", entityType)
        {
        }
    }
}
