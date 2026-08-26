namespace DeepDiff.Exceptions;

public abstract class EntityConfigurationException(string message, Type entityType) : Exception(message)
{
    public Type EntityType { get; } = entityType;
}
