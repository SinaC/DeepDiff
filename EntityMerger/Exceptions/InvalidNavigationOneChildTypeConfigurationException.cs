namespace EntityMerger.Exceptions
{
    public class InvalidNavigationOneChildTypeConfigurationException : MergeEntityConfigurationException
    {
        public string PropertyName { get; }

        public InvalidNavigationOneChildTypeConfigurationException(Type entityType, string propertyName)
            : base($"NavigationOne configuration property {propertyName} for type {entityType} cannot be a collection", entityType)
        {
            PropertyName = propertyName;
        }
    }
}
