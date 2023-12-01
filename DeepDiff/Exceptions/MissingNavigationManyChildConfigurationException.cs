using System;

namespace DeepDiff.Exceptions
{
    public class MissingNavigationManyChildConfigurationException : DiffEntityConfigurationException
    {
        public Type ChildType { get; }

        public MissingNavigationManyChildConfigurationException(Type entityType, Type childType)
            : base($"No configuration found for type {childType} as child type for NavigationMany configuration of type {entityType}", entityType)
        {
            ChildType = childType;
        }
    }
}
