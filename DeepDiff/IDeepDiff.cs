using DeepDiff.Configuration;
using System;
using System.Collections.Generic;

namespace DeepDiff
{
    public interface IDeepDiff
    {
        TEntity DiffSingle<TEntity>(TEntity existingEntity, TEntity newEntity)
            where TEntity : class;

        TEntity DiffSingle<TEntity>(TEntity existingEntity, TEntity newEntity, Action<IDiffSingleConfiguration> diffSingleConfigurationAction)
            where TEntity : class;

        IEnumerable<TEntity> DiffMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
            where TEntity : class;

        IEnumerable<TEntity> DiffMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, Action<IDiffManyConfiguration> diffManyConfigurationAction)
            where TEntity : class;
    }
}