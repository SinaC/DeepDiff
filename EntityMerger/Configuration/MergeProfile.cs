namespace EntityMerger.Configuration
{
    public abstract class MergeProfile
    {
        internal Dictionary<Type, MergeEntityConfiguration> MergeEntityConfigurations { get; private set; } = new Dictionary<Type, MergeEntityConfiguration>();

        protected IMergeEntityConfiguration<TEntity> AddMergeEntityConfiguration<TEntity>()
            where TEntity : class
        {
            var mergeEntityConfiguration = new MergeEntityConfiguration(typeof(TEntity));
            MergeEntityConfigurations.Add(typeof(TEntity), mergeEntityConfiguration);

            return new MergeEntityConfiguration<TEntity>(mergeEntityConfiguration);
        }
    }
}
