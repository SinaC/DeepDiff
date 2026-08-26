namespace DeepDiff.Exceptions;

public class DuplicateKeysException(Type entityType, string keys) : Exception($"Duplicate key(s) found on type {entityType} with key(s) {keys}")
{
    public Type EntityType { get; } = entityType;
    public string Keys { get; } = keys;
}
