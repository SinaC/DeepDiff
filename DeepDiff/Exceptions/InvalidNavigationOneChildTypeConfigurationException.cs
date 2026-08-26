namespace DeepDiff.Exceptions;

public sealed class InvalidNavigationOneChildTypeConfigurationException(Type entityType, string propertyName, string message) : EntityConfigurationException(message, entityType)
{
    public string PropertyName { get; } = propertyName;
}
