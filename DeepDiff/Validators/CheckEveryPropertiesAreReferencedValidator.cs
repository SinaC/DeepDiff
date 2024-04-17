using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Validators
{
    internal class CheckEveryPropertiesAreReferencedValidator
    {
        public IEnumerable<Exception> Validate(Type type, EntityConfiguration entityConfiguration, IEnumerable<string> ignoredPropertyNames, IEnumerable<Type> typesToCheck, bool checkEnum)
        {
            var propertiesToCheck = type.GetProperties().Where(x => x.GetSetMethod(false)?.IsPublic == true && (ignoredPropertyNames == null || !ignoredPropertyNames.Contains(x.Name)) && ((x.PropertyType.IsEnum && checkEnum) || typesToCheck?.Contains(x.PropertyType) == true));
            return Validate(type, entityConfiguration, propertiesToCheck);
        }

        public IEnumerable<Exception> Validate(Type type, EntityConfiguration entityConfiguration)
        {
            var propertiesToCheck = type.GetProperties().Where(x => x.GetSetMethod(false)?.IsPublic == true && (entityConfiguration.IgnoreConfiguration == null || entityConfiguration.IgnoreConfiguration.IgnoredProperties.All(y => !x.IsSameAs(y))));
            return Validate(type, entityConfiguration, propertiesToCheck);
        }

        private IEnumerable<Exception> Validate(Type type, EntityConfiguration entityConfiguration, IEnumerable<PropertyInfo> propertiesToCheck)
        {
            foreach (var property in propertiesToCheck)
            {
                // check key properties
                var found = CheckIfPropertyFound(property, entityConfiguration);
                if (!found)
                    yield return new PropertyNotReferenceInConfigurationException(type, property.Name);
            }
        }

        private bool CheckIfPropertyFound(PropertyInfo property, EntityConfiguration entityConfiguration)
            => entityConfiguration.KeyConfiguration?.KeyProperties?.Contains(property) == true
                    || entityConfiguration.ValuesConfiguration?.ValuesProperties?.Contains(property) == true
                    || entityConfiguration.NavigationManyConfigurations?.Select(x => x.NavigationProperty)?.Contains(property) == true
                    || entityConfiguration.NavigationOneConfigurations?.Select(x => x.NavigationProperty)?.Contains(property) == true
                    || entityConfiguration.UpdateConfiguration?.SetValueConfigurations?.Select(x => x.DestinationProperty)?.Contains(property) == true
                    || entityConfiguration.UpdateConfiguration?.CopyValuesConfiguration?.CopyValuesProperties?.Contains(property) == true
                    || entityConfiguration.InsertConfiguration?.SetValueConfigurations?.Select(x => x.DestinationProperty)?.Contains(property) == true
                    || entityConfiguration.DeleteConfiguration?.SetValueConfigurations?.Select(x => x.DestinationProperty)?.Contains(property) == true;

        
    }
}
