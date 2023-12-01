using System.Collections.Generic;

namespace DeepDiff
{
    public interface IDeepDiff
    {
        IEnumerable<TEntity> Diff<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
            where TEntity : class;

        TEntity Diff<TEntity>(TEntity existingEntity, TEntity newEntity)
            where TEntity : class;
    }
}