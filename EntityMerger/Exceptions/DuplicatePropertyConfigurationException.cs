namespace EntityMerger.Exceptions
{
    public class DuplicatePropertyConfigurationException : MergeEntityConfigurationException
    {
        public string[] DuplicatePropertyNames { get; }

        public DuplicatePropertyConfigurationException(Type entityType, string configurationType, IEnumerable<string> duplicatePropertyNames)
            : base($"{configurationType} configuration for type {entityType} contains one or more duplicated property: {string.Join(",", duplicatePropertyNames)}", entityType)
        {
            DuplicatePropertyNames = duplicatePropertyNames.ToArray();
        }
    }
}
