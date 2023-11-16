namespace EntityMerger;

public interface IMerger
{
    IEnumerable<TEntity> Merge<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> calculatedEntities)
        where TEntity : class;

    bool Equals<TEntity>(TEntity entity1, TEntity entity2);
}
