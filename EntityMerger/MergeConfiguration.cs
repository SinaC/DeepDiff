namespace EntityMerger.EntityMerger;

public class MergeConfiguration
{
    internal Dictionary<Type, MergeEntityConfiguration> MergeEntityConfigurations { get; private set; }

    public MergeConfiguration()
    {
        MergeEntityConfigurations = new Dictionary<Type, MergeEntityConfiguration>();
    }

    public virtual MergeEntityConfiguration<TEntityType> Entity<TEntityType>()
        where TEntityType : PersistEntity
    {
        MergeEntityConfiguration mergeEntityConfiguration = new MergeEntityConfiguration(typeof(TEntityType));
        MergeEntityConfigurations.Add(typeof(TEntityType), mergeEntityConfiguration);

        return new MergeEntityConfiguration<TEntityType>(mergeEntityConfiguration);
    }

    public Merger CreateMerger()
    {
        // TODO: validate configuration (Keys cannot be empty)

        return new Merger(this);
    }
}
