namespace DeepDiff.Exceptions;

public class DuplicateTypeSpecificComparerConfigurationException(Type entityType, Type propertyType) : EntityConfigurationException($"WithConverted<{propertyType}> has already been configured for {entityType}", entityType)
{
}
