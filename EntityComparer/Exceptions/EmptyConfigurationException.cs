namespace EntityComparer.Exceptions
{
    public class EmptyConfigurationException : CompareEntityConfigurationException
    {
        public EmptyConfigurationException(Type entityType, string configurationType)
            : base($"{configurationType} configuration for type {entityType} is empty", entityType)
        {
        }
    }
}
