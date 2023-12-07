using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using System;
using System.Collections.Generic;

namespace DeepDiff.Validators
{
    internal sealed class NavigationManyValidator : ValidatorBase
    {
        // every NavigationManyChildType must exist in configuration
        public override IEnumerable<Exception> Validate(Type entityType, DiffEntityConfiguration diffEntityConfiguration, IReadOnlyDictionary<Type, DiffEntityConfiguration> diffEntityConfigurationByTypes)
        {
            if (diffEntityConfiguration.NavigationManyConfigurations != null)
            {
                foreach (var configuration in diffEntityConfiguration.NavigationManyConfigurations)
                {
                    // check if navigation child type is found in configuration
                    if (!diffEntityConfigurationByTypes.ContainsKey(configuration.NavigationManyChildType))
                        yield return new MissingNavigationManyChildConfigurationException(entityType, configuration.NavigationManyChildType);
                }
            }
        }
    }
}
