namespace DeepDiff.Exceptions;

public class AbstractEntityConfigurationException(Type entityType) : Exception($"Configuration found for abstract type {entityType}")
{
    public Type EntityType { get; } = entityType;
}
