using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepDiff.Validators
{
    internal sealed class UpdateValidator : ValidatorBase
    {
        // cannot be null
        // must contain SetValue
        // if copy values, cannot contain duplicate, cannot be found in key nor values configuration
        public override IEnumerable<Exception> Validate(Type entityType, DiffEntityConfiguration diffEntityConfiguration, IReadOnlyDictionary<Type, DiffEntityConfiguration> diffEntityConfigurationByTypes)
        {
            var updateConfiguration = diffEntityConfiguration.UpdateConfiguration;
            if (updateConfiguration != null)
            {
                // set value
                if (updateConfiguration.SetValueConfiguration == null)
                    yield return new MissingSetValueConfigurationException(entityType, "OnUpdate");

                // copy values
                var copyValuesConfigurations = updateConfiguration.CopyValuesConfiguration;
                if (copyValuesConfigurations != null)
                {
                    // cannot be empty
                    if (copyValuesConfigurations.CopyValuesProperties == null || copyValuesConfigurations.CopyValuesProperties.Count == 0)
                        yield return new EmptyConfigurationException(entityType, NameOf<CopyValuesConfiguration>());
                    else
                    {
                        // cannot contain duplicates
                        var duplicates = copyValuesConfigurations.CopyValuesProperties.FindDuplicate().ToArray();
                        if (duplicates.Length > 0)
                            yield return new DuplicatePropertyConfigurationException(entityType, NameOf<CopyValuesConfiguration>(), duplicates.Select(x => x.Name));
                        // cannot be defined in keys
                        if (diffEntityConfiguration.KeyConfiguration?.KeyProperties != null)
                        {
                            var alreadyDefinedInKey = copyValuesConfigurations.CopyValuesProperties.Intersect(diffEntityConfiguration.KeyConfiguration.KeyProperties).ToArray();
                            if (alreadyDefinedInKey.Length > 0)
                                yield return new AlreadyDefinedPropertyException(entityType, NameOf<CopyValuesConfiguration>(), NameOf<KeyConfiguration>(), alreadyDefinedInKey.Select(x => x.Name));
                        }
                        // cannot be found in values
                        if (diffEntityConfiguration.ValuesConfiguration?.ValuesProperties != null)
                        {
                            var alreadyDefinedInKey = copyValuesConfigurations.CopyValuesProperties.Intersect(diffEntityConfiguration.ValuesConfiguration.ValuesProperties).ToArray();
                            if (alreadyDefinedInKey.Length > 0)
                                yield return new AlreadyDefinedPropertyException(entityType, NameOf<CopyValuesConfiguration>(), NameOf<ValuesConfiguration>(), alreadyDefinedInKey.Select(x => x.Name));
                        }
                    }
                }
            }
        }
    }
}
