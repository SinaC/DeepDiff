namespace DeepDiff.Exceptions;

public sealed class MissingNavigationManyChildConfigurationException(Type entityType, Type childType) : EntityConfigurationException($"No configuration found for type {childType} as child type for NavigationMany configuration of type {entityType}", entityType)
{
    public Type ChildType { get; } = childType;
}
