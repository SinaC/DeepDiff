namespace DeepDiff.Configuration
{
    public interface IValuesConfiguration<TEntity>
        where TEntity : class
    {
        IValuesConfiguration<TEntity> UsePrecompiledEqualityComparer(bool use = true);
    }
}