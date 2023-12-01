using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepDiff.Exceptions
{
    public class AlreadyDefinedPropertyException : DiffEntityConfigurationException
    {
        public string[] AlreadyDefinedPropertyNames { get; }

        public AlreadyDefinedPropertyException(Type entityType, string faultyConfiguration, string alreadyDefinedInConfiguration, IEnumerable<string> alreadyDefinedPropertyNames)
            : base($"{faultyConfiguration} configuration for type {entityType} contains one or more property already configured in {alreadyDefinedInConfiguration}: {string.Join(",", alreadyDefinedPropertyNames)}", entityType)
        {
            AlreadyDefinedPropertyNames = alreadyDefinedPropertyNames.ToArray();
        }
    }
}
