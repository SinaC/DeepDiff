namespace EntityComparer.Exceptions
{
    public class MissingKeyConfigurationException : CompareEntityConfigurationException
    {
        public MissingKeyConfigurationException(Type entityType)
            : base($"No Key configuration has been configured for type {entityType}", entityType)
        {
        }
    }
}
