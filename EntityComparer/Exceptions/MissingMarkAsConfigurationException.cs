using EntityComparer.Configuration;
using System;

namespace EntityComparer.Exceptions
{
    public class MissingMarkAsConfigurationException : CompareEntityConfigurationException
    {
        public MissingMarkAsConfigurationException(Type entityType, string configurationType)
            : base($"No {configurationType} configuration has been configured for type {entityType}", entityType)
        {
        }

        internal MissingMarkAsConfigurationException(Type entityType, CompareEntityOperation compareEntityOperation)
            : this(entityType, NameOf(compareEntityOperation))
        {
        }

        private static string NameOf(CompareEntityOperation compareEntityOperation)
            => compareEntityOperation switch
            {
                CompareEntityOperation.Insert => "MarkAsInserted",
                CompareEntityOperation.Update => "MarkAsUpdated",
                CompareEntityOperation.Delete => "MarkAsDeleted",
                _ => throw new NotImplementedException()
            };
    }
}
