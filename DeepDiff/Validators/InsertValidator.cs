using DeepDiff.Configuration;
using System;
using System.Collections.Generic;

namespace DeepDiff.Validators
{
    internal sealed class InsertValidator : OperationValidatorBase
    {
        protected override string OperationConfigurationName { get; } = "OnInsert";

        // cannot be null
        // must contain SetValue
        // set value cannot be found in key nor values configuration
        public override IEnumerable<Exception> Validate(Type entityType, EntityConfiguration entityConfiguration, IReadOnlyDictionary<Type, EntityConfiguration> entityConfigurationByTypes)
        {
            var insertConfiguration = entityConfiguration.InsertConfiguration;
            if (insertConfiguration != null)
            {
                // set value
                var setValueExceptions = ValidateSetValue(entityType, entityConfiguration, insertConfiguration.SetValueConfiguration);
                foreach (var setValueEception in setValueExceptions)
                    yield return setValueEception;
            }
        }
    }
}
