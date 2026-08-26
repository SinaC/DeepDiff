namespace DeepDiff.Exceptions;

public class PropertyNotReferenceInConfigurationException(Type entityType, string propertyName) : EntityConfigurationException($"Property {propertyName} is not referenced in configuration for {entityType}", entityType)
{
    public string PropertyName { get; } = propertyName;
}
