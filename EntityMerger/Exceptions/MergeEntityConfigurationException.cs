namespace EntityMerger.Exceptions
{
    public class MergeEntityConfigurationException : Exception
    {
        public Type EntityType { get; }

        public MergeEntityConfigurationException(string message, Type entityType)
            : base(message)
        {
            EntityType = entityType;
        }
    }
}
