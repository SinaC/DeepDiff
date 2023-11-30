using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityComparer.Exceptions
{
    public class DuplicatePropertyConfigurationException : CompareEntityConfigurationException
    {
        public string[] DuplicatePropertyNames { get; }

        public DuplicatePropertyConfigurationException(Type entityType, string configurationType, IEnumerable<string> duplicatePropertyNames)
            : base($"{configurationType} configuration for type {entityType} contains one or more duplicated property: {string.Join(",", duplicatePropertyNames)}", entityType)
        {
            DuplicatePropertyNames = duplicatePropertyNames.ToArray();
        }
    }
}
