namespace EntityMerger.Exceptions
{
    public class AlreadyDefinedPropertyException : MergeEntityConfigurationException
    {
        public string[] AlreadyDefinedPropertyNames { get; }

        public AlreadyDefinedPropertyException(Type entityType, string faultyConfigurationType, string alreadyDefinedInConfigurationType, IEnumerable<string> alreadyDefinedPropertyNames)
            : base($"{faultyConfigurationType} configuration for type {entityType} contains one or more property already configured in {alreadyDefinedInConfigurationType}: {string.Join(",", alreadyDefinedPropertyNames)}", entityType)
        {
            AlreadyDefinedPropertyNames = alreadyDefinedPropertyNames.ToArray();
        }
    }
}
