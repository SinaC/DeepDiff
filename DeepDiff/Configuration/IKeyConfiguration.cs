namespace DeepDiff.Configuration
{
    public interface IKeyConfiguration<TEntity>
        where TEntity : class
    {
        IKeyConfiguration<TEntity> UsePrecompiledEqualityComparer(bool use = true);
    }
}