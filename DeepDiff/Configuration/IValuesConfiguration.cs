namespace DeepDiff.Configuration
{
    public interface IValuesConfiguration<TEntity>
        where TEntity : class
    {
        IValuesConfiguration<TEntity> DisablePrecompiledEqualityComparer();
        IValuesConfiguration<TEntity> EnablePrecompiledEqualityComparer();
    }
}