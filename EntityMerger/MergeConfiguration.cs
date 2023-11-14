namespace EntityMerger.EntityMerger;

public class MergeConfiguration : IMergeConfiguration
{
    internal Dictionary<Type, MergeEntityConfiguration> MergeEntityConfigurations { get; private set; }

    public MergeConfiguration()
    {
        MergeEntityConfigurations = new Dictionary<Type, MergeEntityConfiguration>();
    }

    public virtual IMergeEntityConfiguration<TEntityType> Entity<TEntityType>()
        where TEntityType : class
    {
        var mergeEntityConfiguration = new MergeEntityConfiguration(typeof(TEntityType));
        MergeEntityConfigurations.Add(typeof(TEntityType), mergeEntityConfiguration);

        return new MergeEntityConfiguration<TEntityType>(mergeEntityConfiguration);
    }

    public IMerger CreateMerger()
    {
        // TODO: validate configuration (Keys cannot be empty + InsertAssignValue/UpdateAssignValue/DeleteAssignValue cannot be null, ...)

        return new Merger(this);
    }
}
