namespace DeepDiff.Configuration
{
    /// <summary>
    /// Configuration the navigation to child entity.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TChildEntity"></typeparam>
    public interface INavigationOneConfiguration<TEntity, TChildEntity>
        where TEntity : class
        where TChildEntity : class
    {
    }
}