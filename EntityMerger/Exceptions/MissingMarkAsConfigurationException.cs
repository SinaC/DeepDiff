using EntityMerger.Configuration;

namespace EntityMerger.Exceptions
{
    public class MissingMarkAsConfigurationException : MergeEntityConfigurationException
    {
        public MissingMarkAsConfigurationException(Type entityType, string configurationType)
            : base($"No {configurationType} configuration has been configured for type {entityType}", entityType)
        {
        }

        internal MissingMarkAsConfigurationException(Type entityType, MergeEntityOperation mergeEntityOperation)
            : this(entityType, NameOf(mergeEntityOperation))
        {
        }

        private static string NameOf(MergeEntityOperation mergeEntityOperation)
            => mergeEntityOperation switch
            {
                MergeEntityOperation.Insert => "MarkAsInserted",
                MergeEntityOperation.Update => "MarkAsUpdated",
                MergeEntityOperation.Delete => "MarkAsDeleted",
                _ => throw new NotImplementedException()
            };
    }
}
