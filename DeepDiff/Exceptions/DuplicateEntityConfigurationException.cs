namespace DeepDiff.Exceptions;

public sealed class DuplicateEntityConfigurationException(Type entityType) : EntityConfigurationException($"A configuration for {entityType} has already been defined", entityType)
{
}
