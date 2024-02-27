using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepDiff.Exceptions
{
    public class NoKeyButFoundInNavigationManyConfigurationException : EntityConfigurationException
    {
        public Type[] ReferencingEntities { get; }

        public NoKeyButFoundInNavigationManyConfigurationException(Type entityType, IEnumerable<Type> referencingEntityNavigationManyConfigurations)
            : base($"NoKey set to true for {entityType} but found in HasMany of {string.Join(",", referencingEntityNavigationManyConfigurations.Select(x => x.Name))}", entityType)
        {
            ReferencingEntities = referencingEntityNavigationManyConfigurations.ToArray();
        }
    }
}
