using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using System;
using System.Collections.Generic;

namespace DeepDiff.Validators
{
    internal sealed class DeleteValidator : ValidatorBase
    {
        // cannot be null
        // must contain SetValue
        public override IEnumerable<Exception> Validate(Type entityType, DiffEntityConfiguration diffEntityConfiguration, IReadOnlyDictionary<Type, DiffEntityConfiguration> diffEntityConfigurationByTypes)
        {
            var deleteConfiguration = diffEntityConfiguration.DeleteConfiguration;
            if (deleteConfiguration == null)
                yield return new MissingOnDeleteConfigurationException(entityType);
            else
            {
                if (deleteConfiguration.SetValueConfiguration == null)
                    yield return new MissingSetValueConfigurationException(entityType, "OnDelete");
            }
        }
    }
}
