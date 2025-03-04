using DeepDiff.Configuration;
using System;
using System.Collections.Generic;

namespace DeepDiff
{
    public interface IDeepDiff
    {
        TEntity MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity)
            where TEntity : class;

        TEntity MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener operationListener)
            where TEntity : class;

        TEntity MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity, Action<IMergeSingleConfiguration> mergeSingleConfigurationAction)
            where TEntity : class;

        TEntity MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener operationListener, Action<IMergeSingleConfiguration> mergeSingleConfigurationAction)
            where TEntity : class;

        IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
            where TEntity : class;

        IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener operationListener)
            where TEntity : class;

        IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, Action<IMergeManyConfiguration> mergeManyConfigurationAction)
            where TEntity : class;

        IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener operationListener, Action<IMergeManyConfiguration> mergeManyConfigurationAction)
            where TEntity : class;

        void CompareSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener operationListener)
            where TEntity : class;

        void CompareSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener operationListener, Action<ICompareSingleConfiguration> diffSingleConfigurationAction)
            where TEntity : class;

        void CompareMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener operationListener)
            where TEntity : class;

        void CompareMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener operationListener, Action<ICompareManyConfiguration> diffManyConfigurationAction)
            where TEntity : class;
    }
}