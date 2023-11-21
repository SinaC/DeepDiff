namespace EntityMerger.Exceptions
{
    public class EmptyConfigurationException : MergeEntityConfigurationException
    {
        public EmptyConfigurationException(Type entityType, string configurationType)
            : base($"{configurationType} configuration for type {entityType} is empty", entityType)
        {
        }
    }
}
