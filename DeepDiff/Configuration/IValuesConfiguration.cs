namespace DeepDiff.Configuration
{
    /// <summary>
    /// Specify the value(s) for entity. Will be used to detect update operations.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IValuesConfiguration<TEntity>
        where TEntity : class
    {
    }
}