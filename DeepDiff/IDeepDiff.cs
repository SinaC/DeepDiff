using System.Collections.Generic;

namespace DeepDiff
{
    public interface IDeepDiff
    {
        IEnumerable<TEntity> DiffMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
            where TEntity : class;

        TEntity DiffSingle<TEntity>(TEntity existingEntity, TEntity newEntity)
            where TEntity : class;
    }
}