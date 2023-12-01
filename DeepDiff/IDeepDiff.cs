using System.Collections.Generic;

namespace DeepDiff
{
    public interface IDeepDiff
    {
        IEnumerable<TEntity> Diff<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
            where TEntity : class;
    }
}