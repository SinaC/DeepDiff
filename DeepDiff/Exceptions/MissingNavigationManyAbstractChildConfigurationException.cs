namespace DeepDiff.Exceptions;

public sealed class MissingNavigationManyAbstractChildConfigurationException(Type entityType, Type childType) : EntityConfigurationException($"No configuration found for at least one type derived from {childType} as child type for NavigationMany configuration of type {entityType}", entityType)
{
    public Type ChildType { get; } = childType;
}
