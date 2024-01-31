using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using System;
using System.Collections.Generic;

namespace DeepDiff.Validators
{
    internal sealed class InsertValidator : ValidatorBase
    {
        // cannot be null
        // must contain SetValue
        public override IEnumerable<Exception> Validate(Type entityType, EntityConfiguration entityConfiguration, IReadOnlyDictionary<Type, EntityConfiguration> entityConfigurationByTypes)
        {
            var insertConfiguration = entityConfiguration.InsertConfiguration;
            if (insertConfiguration != null)
            {
                if (insertConfiguration.SetValueConfiguration == null)
                    yield return new MissingSetValueConfigurationException(entityType, "OnInsert");
            }
        }
    }
}
