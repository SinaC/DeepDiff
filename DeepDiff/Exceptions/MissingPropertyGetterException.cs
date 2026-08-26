namespace DeepDiff.Exceptions;

public class MissingPropertyGetterException(Type entityType, string propertyName) : EntityConfigurationException($"Property {propertyName} of type {entityType} does not have a getter and cannot be used in configuration", entityType)
{
    public string PropertyName { get; } = propertyName;
}
