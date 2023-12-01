using DeepDiff.Configuration;
using System;

namespace DeepDiff.Exceptions
{
    public class MissingMarkAsConfigurationException : DiffEntityConfigurationException
    {
        public MissingMarkAsConfigurationException(Type entityType, string configurationType)
            : base($"No {configurationType} configuration has been configured for type {entityType}", entityType)
        {
        }

        internal MissingMarkAsConfigurationException(Type entityType, DiffEntityOperation diffEntityOperation)
            : this(entityType, NameOf(diffEntityOperation))
        {
        }

        private static string NameOf(DiffEntityOperation diffEntityOperation)
            => diffEntityOperation switch
            {
                DiffEntityOperation.Insert => "MarkAsInserted",
                DiffEntityOperation.Update => "MarkAsUpdated",
                DiffEntityOperation.Delete => "MarkAsDeleted",
                _ => throw new NotImplementedException()
            };
    }
}
