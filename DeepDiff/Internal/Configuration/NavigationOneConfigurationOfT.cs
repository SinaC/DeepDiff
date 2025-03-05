using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class NavigationOneConfiguration<TEntity, TChildEntity> : INavigationOneConfiguration<TEntity, TChildEntity>
        where TEntity : class
        where TChildEntity : class
    {
        private NavigationOneConfiguration Configuration { get; } // cannot be removed because it will be used in the future when methods will be added in INavigationOneConfiguration

        public NavigationOneConfiguration(NavigationOneConfiguration navigationOneConfiguration)
        {
            Configuration = navigationOneConfiguration;
        }
    }
}