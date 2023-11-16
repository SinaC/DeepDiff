namespace EntityMerger.EntityMerger;

public interface IMergeConfiguration
{
    IMergeEntityConfiguration<TEntity> Entity<TEntity>()
        where TEntity : class;

    IMerger CreateMerger();
    IHashMerger CreateHashMerger();
}
