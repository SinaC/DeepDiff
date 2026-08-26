namespace DeepDiff.Exceptions;

public class NoKeyEntityInNavigationManyException(Type entityType) : Exception($"NoKey set on type {entityType} but used in a HasMany configuration")
{
    public Type EntityType { get; } = entityType;
}
