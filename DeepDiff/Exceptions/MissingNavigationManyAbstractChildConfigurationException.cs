using System;

namespace DeepDiff.Exceptions
{
    public sealed class MissingNavigationManyAbstractChildConfigurationException : EntityConfigurationException
    {
        public Type ChildType { get; }

        public MissingNavigationManyAbstractChildConfigurationException(Type entityType, Type childType)
            : base($"No configuration found for at least one type derived from {childType} as child type for NavigationMany configuration of type {entityType}", entityType)
        {
            ChildType = childType;
        }
    }
}
