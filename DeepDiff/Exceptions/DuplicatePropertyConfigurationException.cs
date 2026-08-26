namespace DeepDiff.Exceptions;

public sealed class DuplicatePropertyConfigurationException(Type entityType, string configurationType, IEnumerable<string> duplicatePropertyNames) : EntityConfigurationException($"{configurationType} configuration for type {entityType} contains one or more duplicated property: {string.Join(",", duplicatePropertyNames)}", entityType)
{
    public string[] DuplicatePropertyNames { get; } = duplicatePropertyNames.ToArray();
}
