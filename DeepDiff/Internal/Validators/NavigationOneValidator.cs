﻿using DeepDiff.Exceptions;
using DeepDiff.Internal.Configuration;
using DeepDiff.Internal.Extensions;
using System;
using System.Collections.Generic;

namespace DeepDiff.Internal.Validators
{
    internal sealed class NavigationOneValidator : ValidatorBase
    {
        // every NavigationOneProperty cannot be a collection and must exist in configuration
        public override IEnumerable<Exception> Validate(Type entityType, EntityConfiguration entityConfiguration, IReadOnlyDictionary<Type, EntityConfiguration> entityConfigurationByTypes)
        {
            if (entityConfiguration.NavigationOneConfigurations != null)
            {
                foreach (var configuration in entityConfiguration.NavigationOneConfigurations)
                {
                    // check if navigation one property is not abstract
                    if (configuration.NavigationChildType.IsAbstract)
                        yield return new InvalidNavigationOneChildTypeConfigurationException(entityType, configuration.NavigationProperty.Name, $"NavigationOne configuration property {configuration.NavigationProperty.Name} for type {entityType} cannot be an abstract type");
                    // check if navigation one property is not a collection
                    else if (configuration.NavigationProperty.IsEnumerable())
                        yield return new InvalidNavigationOneChildTypeConfigurationException(entityType, configuration.NavigationProperty.Name, $"NavigationOne configuration property {configuration.NavigationProperty.Name} for type {entityType} cannot be a collection");
                    else
                    {
                        // check if navigation child type is found in configuration
                        if (!entityConfigurationByTypes.ContainsKey(configuration.NavigationChildType))
                            yield return new MissingNavigationOneChildConfigurationException(entityType, configuration.NavigationChildType);
                    }
                }
            }
        }
    }
}
