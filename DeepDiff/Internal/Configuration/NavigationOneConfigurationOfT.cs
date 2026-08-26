using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration;

internal sealed class NavigationOneConfiguration<TEntity, TChildEntity>(NavigationOneConfiguration navigationOneConfiguration) : INavigationOneConfiguration<TEntity, TChildEntity>
    where TEntity : class
    where TChildEntity : class
{
    private NavigationOneConfiguration Configuration { get; } = navigationOneConfiguration;
}