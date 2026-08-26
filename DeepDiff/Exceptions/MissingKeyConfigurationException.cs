namespace DeepDiff.Exceptions;

public sealed class MissingKeyConfigurationException(Type entityType) : EntityConfigurationException($"No Key configuration has been configured for type {entityType}", entityType)
{
}
