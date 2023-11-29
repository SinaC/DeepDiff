namespace EntityComparer;

public interface IEntityComparer
{
    IEnumerable<TEntity> Compare<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
        where TEntity : class;
}
