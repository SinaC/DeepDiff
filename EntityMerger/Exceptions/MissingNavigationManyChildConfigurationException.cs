namespace EntityMerger.Exceptions
{
    public class MissingNavigationManyChildConfigurationException : MergeEntityConfigurationException
    {
        public Type TargetType { get; }

        public MissingNavigationManyChildConfigurationException(Type entityType, Type targetType)
            : base($"No configuration found for type {targetType} as target type for NavigationManyConfiguration of type {entityType}", entityType)
        {
            TargetType = targetType;
        }
    }
}
