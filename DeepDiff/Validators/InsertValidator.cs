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
        public override IEnumerable<Exception> Validate(Type entityType, DiffEntityConfiguration diffEntityConfiguration, IReadOnlyDictionary<Type, DiffEntityConfiguration> diffEntityConfigurationByTypes)
        {
            var insertConfiguration = diffEntityConfiguration.InsertConfiguration;
            if (insertConfiguration != null)
            {
                if (insertConfiguration.SetValueConfiguration == null)
                    yield return new MissingSetValueConfigurationException(entityType, "OnInsert");
            }
        }
    }
}
