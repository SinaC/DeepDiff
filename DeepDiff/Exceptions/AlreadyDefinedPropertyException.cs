namespace DeepDiff.Exceptions;

public sealed class AlreadyDefinedPropertyException(Type entityType, string faultyConfiguration, string alreadyDefinedInConfiguration, IEnumerable<string> alreadyDefinedPropertyNames) : EntityConfigurationException($"{faultyConfiguration} configuration for type {entityType} contains one or more property already configured in {alreadyDefinedInConfiguration}: {string.Join(",", alreadyDefinedPropertyNames)}", entityType)
{
    public string[] AlreadyDefinedPropertyNames { get; } = alreadyDefinedPropertyNames.ToArray();
}
