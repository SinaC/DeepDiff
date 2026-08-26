namespace DeepDiff.Exceptions;

public sealed class EmptyConfigurationException(Type entityType, string configurationType) : EntityConfigurationException($"{configurationType} configuration for type {entityType} is empty", entityType)
{
}
