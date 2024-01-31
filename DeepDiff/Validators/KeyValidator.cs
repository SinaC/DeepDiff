using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepDiff.Validators
{
    internal sealed class KeyValidator : ValidatorBase
    {
        // cannot be null or empty
        // every property must be different
        public override IEnumerable<Exception> Validate(Type entityType, EntityConfiguration entityConfiguration, IReadOnlyDictionary<Type, EntityConfiguration> entityConfigurationByTypes)
        {
            var configuration = entityConfiguration.KeyConfiguration;
            // cannot be null
            if (configuration == null)
                yield return new MissingKeyConfigurationException(entityType);
            else
            {
                // cannot be empty
                if (configuration.KeyProperties == null || configuration.KeyProperties.Count == 0)
                    yield return new EmptyConfigurationException(entityType, NameOf<KeyConfiguration>());
                else
                {
                    // cannot contain duplicates
                    var duplicates = configuration.KeyProperties.FindDuplicate().ToArray();
                    if (duplicates.Length > 0)
                        yield return new DuplicatePropertyConfigurationException(entityType, NameOf<KeyConfiguration>(), duplicates.Select(x => x.Name));
                }
            }
        }
    }
}
