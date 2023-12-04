using System.Collections.Generic;

namespace DeepDiff
{
    public interface IDeepDiff
    {
        TEntity DiffSingle<TEntity>(TEntity existingEntity, TEntity newEntity)
            where TEntity : class;

        IEnumerable<TEntity> DiffMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
            where TEntity : class;
    }
}