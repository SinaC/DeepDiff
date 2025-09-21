using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.Internal.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("DeepDiff.UnitTest")]
[assembly: InternalsVisibleTo("DeepDiff.PerformanceTest")]

namespace DeepDiff.Internal
{
    internal sealed class DeepDiff : IDeepDiff
    {
        private DeepDiffConfiguration DeepDiffConfiguration { get; }

        internal DeepDiff(DeepDiffConfiguration deepDiffConfiguration)
        {
            DeepDiffConfiguration = deepDiffConfiguration;
        }

        public TEntity? MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity)
            where TEntity : class
            => MergeSingle(existingEntity, newEntity, null, null);

        public TEntity? MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener? operationListener)
            where TEntity : class
            => MergeSingle(existingEntity, newEntity, operationListener, null);

        public TEntity? MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity, Action<IMergeSingleConfiguration>? mergeSingleConfigurationAction)
            where TEntity : class
            => MergeSingle(existingEntity, newEntity, null, mergeSingleConfigurationAction);

        public TEntity? MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener? operationListener, Action<IMergeSingleConfiguration>? mergeSingleConfigurationAction)
            where TEntity : class
        {
            var entityType = typeof(TEntity);
            if (!DeepDiffConfiguration.EntityConfigurationByTypes.TryGetValue(entityType, out var entityConfiguration))
                throw new MissingConfigurationException(entityType);

            var mergeSingleConfiguration = new MergeSingleConfiguration();
            mergeSingleConfigurationAction?.Invoke(mergeSingleConfiguration);

            var mergedEntity = DeepDiffEngine.MergeSingle(DeepDiffConfiguration.EntityConfigurationByTypes, mergeSingleConfiguration.Configuration, operationListener, entityConfiguration, existingEntity, newEntity);
            return mergedEntity as TEntity;
        }

        public IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
            where TEntity : class
            => MergeMany(existingEntities, newEntities, null, null);

        public IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener? operationListener)
            where TEntity : class
            => MergeMany(existingEntities, newEntities, operationListener, null);


        public IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, Action<IMergeManyConfiguration>? mergeManyConfigurationAction)
            where TEntity : class
            => MergeMany(existingEntities, newEntities, null, mergeManyConfigurationAction);

        public IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener? operationListener, Action<IMergeManyConfiguration>? mergeManyConfigurationAction)
            where TEntity : class
        {
            var entityType = typeof(TEntity);
            if (!DeepDiffConfiguration.EntityConfigurationByTypes.TryGetValue(entityType, out var entityConfiguration))
                throw new MissingConfigurationException(entityType);

            var mergeManyConfiguration = new MergeManyConfiguration();
            mergeManyConfigurationAction?.Invoke(mergeManyConfiguration);

            var mergedEntities = DeepDiffEngine.MergeMany(DeepDiffConfiguration.EntityConfigurationByTypes, mergeManyConfiguration.Configuration, operationListener, entityConfiguration, existingEntities, newEntities);
            return mergedEntities.Cast<TEntity>();
        }

        public void CompareSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener operationListener)
           where TEntity : class
            => CompareSingle(existingEntity, newEntity, operationListener, null);

        public void CompareSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener operationListener, Action<ICompareSingleConfiguration>? diffSingleConfigurationAction)
            where TEntity : class
        {
            if (operationListener == null)
                throw new CompareWithoutOperationListenerException();

            var entityType = typeof(TEntity);
            if (!DeepDiffConfiguration.EntityConfigurationByTypes.TryGetValue(entityType, out var entityConfiguration))
                throw new MissingConfigurationException(entityType);

            var diffSingleConfiguration = new CompareSingleConfiguration();
            diffSingleConfigurationAction?.Invoke(diffSingleConfiguration);

            DeepDiffEngine.MergeSingle(DeepDiffConfiguration.EntityConfigurationByTypes, diffSingleConfiguration.Configuration, operationListener, entityConfiguration, existingEntity, newEntity);
        }

        public void CompareMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener operationListener)
            where TEntity : class
            => CompareMany(existingEntities, newEntities, operationListener, null);

        public void CompareMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener operationListener, Action<ICompareManyConfiguration>? diffManyConfigurationAction)
            where TEntity : class
        {
            if (operationListener == null)
                throw new CompareWithoutOperationListenerException();

            var entityType = typeof(TEntity);
            if (!DeepDiffConfiguration.EntityConfigurationByTypes.TryGetValue(entityType, out var entityConfiguration))
                throw new MissingConfigurationException(entityType);

            var diffManyConfiguration = new CompareManyConfiguration();
            diffManyConfigurationAction?.Invoke(diffManyConfiguration);

            DeepDiffEngine.MergeMany(DeepDiffConfiguration.EntityConfigurationByTypes, diffManyConfiguration.Configuration, operationListener, entityConfiguration, existingEntities, newEntities);
        }
    }
}