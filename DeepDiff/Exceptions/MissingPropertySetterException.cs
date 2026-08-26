namespace DeepDiff.Exceptions;

public class MissingPropertySetterException(Type entityType, string propertyName) : EntityConfigurationException($"Property {propertyName} of type {entityType} does not have a setter and cannot be used in configuration", entityType)
{
    public string PropertyName { get; } = propertyName;
}
