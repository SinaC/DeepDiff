using System;

namespace DeepDiff.Exceptions
{
    public sealed class InvalidNavigationOneChildTypeConfigurationException : EntityConfigurationException
    {
        public string PropertyName { get; }

        public InvalidNavigationOneChildTypeConfigurationException(Type entityType, string propertyName, string message)
            : base(message, entityType)
        {
            PropertyName = propertyName;
        }
    }
}
