namespace DeepDiff.Configuration
{
    /// <summary>
    /// Configuration entity key(s). Will be used to detect insert and delete operations.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IKeyConfiguration<TEntity>
        where TEntity : class
    {
    }
}