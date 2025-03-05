using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class NavigationManyConfiguration<TEntity, TChildEntity> : INavigationManyConfiguration<TEntity, TChildEntity>
        where TEntity : class
        where TChildEntity : class
    {
        private NavigationManyConfiguration Configuration { get; }

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