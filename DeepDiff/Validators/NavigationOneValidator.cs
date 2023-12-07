using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.Extensions;
using System;
using System.Collections.Generic;

namespace DeepDiff.Validators
{
    internal sealed class NavigationOneValidator : ValidatorBase
    {
        // every NavigationOneProperty cannot be a collection and must exist in configuration
        public override IEnumerable<Exception> Validate(Type entityType, DiffEntityConfiguration diffEntityConfiguration, IReadOnlyDictionary<Type, DiffEntityConfiguration> diffEntityConfigurationByTypes)
        {
            if (diffEntityConfiguration.NavigationOneConfigurations != null)
            {
                foreach (var configuration in diffEntityConfiguration.NavigationOneConfigurations)
                {
                    // check if navigation one property is not a collection
                    if (configuration.NavigationOneProperty.IsEnumerable())
                        yield return new InvalidNavigationOneChildTypeConfigurationException(entityType, configuration.NavigationOneProperty.Name);
                    else
                    {
                        // check if navigation child type is found in configuration
                        if (!diffEntityConfigurationByTypes.ContainsKey(configuration.NavigationOneChildType))
                            yield return new MissingNavigationOneChildConfigurationException(entityType, configuration.NavigationOneChildType);
                    }
                }
            }
        }
    }
}
