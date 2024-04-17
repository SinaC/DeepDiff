using System;

namespace DeepDiff.Exceptions
{
    public class PropertyNotReferenceInConfigurationException : EntityConfigurationException
    {
        public string PropertyName { get; }

        public PropertyNotReferenceInConfigurationException(Type entityType, string propertyName)
            : base($"Property {propertyName} is not referenced in configuration for {entityType}", entityType)
        {
            PropertyName = propertyName;
        }
    }
}
