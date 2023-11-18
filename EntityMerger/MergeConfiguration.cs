namespace EntityMerger.EntityMerger;

public class MergeConfiguration : IMergeConfiguration
{
    internal Dictionary<Type, MergeEntityConfiguration> MergeEntityConfigurations { get; private set; } = new Dictionary<Type, MergeEntityConfiguration>();
    internal bool UseHashtable { get; private set; } = true;

    public virtual IMergeEntityConfiguration<TEntity> Entity<TEntity>()
        where TEntity : class
    {
        var mergeEntityConfiguration = new MergeEntityConfiguration(typeof(TEntity));
        MergeEntityConfigurations.Add(typeof(TEntity), mergeEntityConfiguration);

        return new MergeEntityConfiguration<TEntity>(mergeEntityConfiguration);
    }

    public IMergeConfiguration DisableHashtable()
    {
        UseHashtable = false;
        return this;
    }

    public IMerger CreateMerger()
    {
        // TODO: validate configuration (Keys cannot be empty + InsertAssignValue/UpdateAssignValue/DeleteAssignValue cannot be null, ...)

        return new Merger(this);
    }
}
