namespace DeepDiff.Exceptions;

public sealed class MissingConfigurationException(Type entityType) : Exception($"No configuration found for type {entityType}")
{
    public Type EntityType { get; } = entityType;
}
