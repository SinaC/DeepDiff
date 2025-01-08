namespace DeepDiff.Configuration
{
    internal sealed class NavigationManyConfiguration<TEntity, TChildEntity> : INavigationManyConfiguration<TEntity, TChildEntity>
        where TEntity : class
        where TChildEntity : class
    {
        public NavigationManyConfiguration Configuration { get; private set; }

        public NavigationManyConfiguration(NavigationManyConfiguration navigationManyConfiguration)
        {
            Configuration = navigationManyConfiguration;
        }

        public INavigationManyConfiguration<TEntity, TChildEntity> UseDerivedTypes(bool useDerivedTypes = false)
        {
            Configuration.SetUseDerivedTypes(useDerivedTypes);
            return this;
        }
    }
}