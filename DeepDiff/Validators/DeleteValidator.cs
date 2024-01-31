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
        public override IEnumerable<Exception> Validate(Type entityType, EntityConfiguration entityConfiguration, IReadOnlyDictionary<Type, EntityConfiguration> entityConfigurationByTypes)
        {
            var deleteConfiguration = entityConfiguration.DeleteConfiguration;
            if (deleteConfiguration != null)
            {
                if (deleteConfiguration.SetValueConfiguration == null)
                    yield return new MissingSetValueConfigurationException(entityType, "OnDelete");
            }
        }
    }
}
