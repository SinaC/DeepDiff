namespace DeepDiff.Exceptions;

public sealed class MissingNavigationOneChildConfigurationException(Type entityType, Type targetType) : EntityConfigurationException($"No configuration found for type {targetType} as target type for NavigationOneConfiguration of type {entityType}", entityType)
{
    public Type TargetType { get; } = targetType;
}
