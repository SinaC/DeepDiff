using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepDiff.Validators
{
    internal abstract class OperationValidatorBase : ValidatorBase
    {
        protected abstract string OperationConfigurationName { get; }

        protected IEnumerable<Exception> ValidateSetValue(Type entityType, EntityConfiguration entityConfiguration, SetValueConfiguration setValueConfiguration)
        {
            if (setValueConfiguration == null)
                yield return new MissingSetValueConfigurationException(entityType, OperationConfigurationName);
            else
            {
                // cannot be defined in keys
                if (entityConfiguration.KeyConfiguration?.KeyProperties != null)
                {
                    var alreadyDefinedInKey = entityConfiguration.KeyConfiguration.KeyProperties.Contains(setValueConfiguration.DestinationProperty);
                    if (alreadyDefinedInKey)
                        yield return new AlreadyDefinedPropertyException(entityType, NameOf<SetValueConfiguration>(OperationConfigurationName), NameOf<KeyConfiguration>(), new[] { setValueConfiguration.DestinationProperty.Name });
                }
                // cannot be found in values
                if (entityConfiguration.ValuesConfiguration?.ValuesProperties != null)
                {
                    var alreadyDefinedInValues = entityConfiguration.ValuesConfiguration.ValuesProperties.Contains(setValueConfiguration.DestinationProperty);
                    if (alreadyDefinedInValues)
                        yield return new AlreadyDefinedPropertyException(entityType, NameOf<SetValueConfiguration>(OperationConfigurationName), NameOf<ValuesConfiguration>(), new[] { setValueConfiguration.DestinationProperty.Name });
                }
            }
        }
    }
}
