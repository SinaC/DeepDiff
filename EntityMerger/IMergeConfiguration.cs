namespace EntityMerger.EntityMerger;

public interface IMergeConfiguration
{
    IMergeEntityConfiguration<TEntityType> Entity<TEntityType>()
        where TEntityType : class;

    IMerger CreateMerger();
}
