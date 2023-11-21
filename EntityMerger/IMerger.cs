namespace EntityMerger;

public interface IMerger
{
    IEnumerable<TEntity> Merge<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
        where TEntity : class;
}
