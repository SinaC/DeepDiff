namespace EntityMerger.EntityMerger;

public class MergeConfiguration : IMergeConfiguration
{
    internal Dictionary<Type, MergeEntityConfiguration> MergeEntityConfigurations { get; private set; }

    public MergeConfiguration()
    {
        MergeEntityConfigurations = new Dictionary<Type, MergeEntityConfiguration>();
    }

    public virtual IMergeEntityConfiguration<TEntity> Entity<TEntity>()
        where TEntity : class
    {
        var mergeEntityConfiguration = new MergeEntityConfiguration(typeof(TEntity));
        MergeEntityConfigurations.Add(typeof(TEntity), mergeEntityConfiguration);

        return new MergeEntityConfiguration<TEntity>(mergeEntityConfiguration);
    }

    public IMerger CreateMerger()
    {
        // TODO: validate configuration (Keys cannot be empty + InsertAssignValue/UpdateAssignValue/DeleteAssignValue cannot be null, ...)

        return new Merger(this);
    }
}
