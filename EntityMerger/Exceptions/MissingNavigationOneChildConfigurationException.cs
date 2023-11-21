namespace EntityMerger.Exceptions
{
    public class MissingNavigationOneChildConfigurationException : MergeEntityConfigurationException
    {
        public Type TargetType { get; }

        public MissingNavigationOneChildConfigurationException(Type entityType, Type targetType)
            : base($"No configuration found for type {targetType} as target type for NavigationOneConfiguration of type {entityType}", entityType)
        {
            TargetType = targetType;
        }
    }
}
