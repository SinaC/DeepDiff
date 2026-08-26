using System.Reflection;

namespace DeepDiff.Exceptions;

public class DuplicatePropertySpecificComparerConfigurationException(Type entityType, PropertyInfo propertyInfo) : EntityConfigurationException($"WithConverted(x => {propertyInfo.Name}) has already been configured for {entityType}", entityType)
{
}
