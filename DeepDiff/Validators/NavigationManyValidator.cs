using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepDiff.Validators
{
    internal sealed class NavigationManyValidator : ValidatorBase
    {
        // every NavigationManyChildType must exist in configuration
        public override IEnumerable<Exception> Validate(Type entityType, EntityConfiguration entityConfiguration, IReadOnlyDictionary<Type, EntityConfiguration> entityConfigurationByTypes)
        {
            if (entityConfiguration.NavigationManyConfigurations != null)
            {
                foreach (var configuration in entityConfiguration.NavigationManyConfigurations)
                {
                    // if UseDerivedTypes has been manually specified or forced by an abstract child entity type
                    if (configuration.UseDerivedTypes)
                    {
                        //  check if there is at least one configuration with a derived class
                        if (!entityConfigurationByTypes.Any(x => x.Key.IsSubclassOf(configuration.NavigationChildType)))
                            yield return new MissingNavigationManyAbstractChildConfigurationException(entityType, configuration.NavigationChildType); // TODO more specific exception
                    }
                    // otherwise
                    else
                    {
                        //  check if navigation child type is found in configuration
                        if (!entityConfigurationByTypes.ContainsKey(configuration.NavigationChildType))
                            yield return new MissingNavigationManyChildConfigurationException(entityType, configuration.NavigationChildType);
                    }
                }
            }
        }
    }
}
