namespace EntityMerger.Exceptions
{
    public class MissingKeyConfigurationException : MergeEntityConfigurationException
    {
        public MissingKeyConfigurationException(Type entityType)
            : base($"No Key configuration has been configured for type {entityType}", entityType)
        {
        }
    }
}
