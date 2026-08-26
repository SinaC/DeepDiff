using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration;

internal sealed class NavigationManyConfiguration<TEntity, TChildEntity>(NavigationManyConfiguration navigationManyConfiguration) : INavigationManyConfiguration<TEntity, TChildEntity>
    where TEntity : class
    where TChildEntity : class
{
    private NavigationManyConfiguration Configuration { get; } = navigationManyConfiguration;

    public INavigationManyConfiguration<TEntity, TChildEntity> UseDerivedTypes(bool useDerivedTypes = false)
    {
        Configuration.SetUseDerivedTypes(useDerivedTypes);
        return this;
    }
}