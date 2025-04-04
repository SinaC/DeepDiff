namespace DeepDiff.Configuration
{
    /// <summary>
    /// Configuration the navigation to child entities.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TChildEntity"></typeparam>
    public interface INavigationManyConfiguration<TEntity, TChildEntity>
        where TEntity : class
        where TChildEntity : class
    {
        /// <summary>
        /// When set to true, engine will use force comparison by type if children are inherited and children collection is not abstract.
        /// </summary>
        /// <param name="use"></param>
        /// <remarks>default value for <paramref name="use"/> is <c>false</c></remarks>
        /// <returns></returns>
        INavigationManyConfiguration<TEntity, TChildEntity> UseDerivedTypes(bool use = false);
    }
}