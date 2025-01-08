namespace DeepDiff.Configuration
{
    public interface INavigationManyConfiguration<TEntity, TChildEntity>
        where TEntity : class
        where TChildEntity : class
    {
        INavigationManyConfiguration<TEntity, TChildEntity> UseDerivedTypes(bool use = false);
    }
}