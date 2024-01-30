using System;
using System.Reflection;

namespace DeepDiff.Exceptions
{
    public class DuplicatePropertySpecificComparerConfigurationException : DiffEntityConfigurationException
    {
        public DuplicatePropertySpecificComparerConfigurationException(Type entityType, PropertyInfo propertyInfo)
            : base($"WithConverted(x => {propertyInfo.Name}) has already been configured for {entityType}", entityType)
        {
        }
    }
}
