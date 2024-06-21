using DeepDiff.Configuration;
using DeepDiff.Operations;
using System;
using System.Collections.Generic;

namespace DeepDiff
{
    public interface IDeepDiff
    {
        MergeSingleResult<TEntity> MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity)
            where TEntity : class;

        MergeSingleResult<TEntity> MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity, Action<IMergeSingleConfiguration> mergeSingleConfigurationAction)
            where TEntity : class;

        MergeManyResult<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
            where TEntity : class;

        MergeManyResult<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, Action<IMergeManyConfiguration> mergeManyConfigurationAction)
            where TEntity : class;

        IReadOnlyCollection<DiffOperationBase> DiffSingle<TEntity>(TEntity existingEntity, TEntity newEntity)
            where TEntity : class;

        IReadOnlyCollection<DiffOperationBase> DiffSingle<TEntity>(TEntity existingEntity, TEntity newEntity, Action<IDiffSingleConfiguration> diffSingleConfigurationAction)
            where TEntity : class;

        IReadOnlyCollection<DiffOperationBase> DiffMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
            where TEntity : class;

        IReadOnlyCollection<DiffOperationBase> DiffMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, Action<IDiffManyConfiguration> diffManyConfigurationAction)
            where TEntity : class;
    }
}