namespace DeepDiff.Configuration
{
    public interface IKeyConfiguration<TEntity>
        where TEntity : class
    {
        IKeyConfiguration<TEntity> DisablePrecompiledEqualityComparer();
        IKeyConfiguration<TEntity> EnablePrecompiledEqualityComparer();
    }
}