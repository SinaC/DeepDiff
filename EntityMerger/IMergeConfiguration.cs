namespace EntityMerger.EntityMerger;

public interface IMergeConfiguration
{
    IMergeEntityConfiguration<TEntity> Entity<TEntity>()
        where TEntity : class;

    IMergeConfiguration DisableHashtable();

    IMerger CreateMerger();
}
