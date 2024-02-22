﻿using DeepDiff.Configuration;
using System;
using System.Collections.Generic;

namespace DeepDiff.Validators
{
    internal sealed class DeleteValidator : OperationValidatorBase
    {
        protected override string OperationConfigurationName { get; } = "OnDelete";

        // cannot be null
        // must contain SetValue
        // set value cannot be found in key nor values configuration
        public override IEnumerable<Exception> Validate(Type entityType, EntityConfiguration entityConfiguration, IReadOnlyDictionary<Type, EntityConfiguration> entityConfigurationByTypes)
        {
            var deleteConfiguration = entityConfiguration.DeleteConfiguration;
            if (deleteConfiguration != null)
            {
                // set value
                var setValueExceptions = ValidateSetValue(entityType, entityConfiguration, deleteConfiguration.SetValueConfiguration);
                foreach (var setValueEception in setValueExceptions)
                    yield return setValueEception;
            }
        }
    }
}
