namespace EntityMerger.Exceptions
{
    public class AlreadyDefinedPropertyException : MergeEntityConfigurationException
    {
        public string[] AlreadyDefinedPropertyNames { get; }

        public AlreadyDefinedPropertyException(Type entityType, string faultyConfiguration, string alreadyDefinedInConfiguration, IEnumerable<string> alreadyDefinedPropertyNames)
            : base($"{faultyConfiguration} configuration for type {entityType} contains one or more property already configured in {alreadyDefinedInConfiguration}: {string.Join(",", alreadyDefinedPropertyNames)}", entityType)
        {
            AlreadyDefinedPropertyNames = alreadyDefinedPropertyNames.ToArray();
        }
    }
}
