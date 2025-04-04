using DeepDiff.Configuration;
using System;
using System.Collections.Generic;

namespace DeepDiff
{
    public interface IDeepDiff
    {
        /// <summary>
        /// Perform a merge on a single entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="existingEntity"></param>
        /// <param name="newEntity"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.DuplicateKeysException"></exception>
        /// <exception cref="Exceptions.MissingConfigurationException"></exception>
        /// <exception cref="Exceptions.NoKeyEntityInNavigationManyException"></exception>
        TEntity MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity)
            where TEntity : class;

        /// <summary>
        /// Perform a merge on a single entity with an operation listener.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="existingEntity"></param>
        /// <param name="newEntity"></param>
        /// <param name="operationListener"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.DuplicateKeysException"></exception>
        /// <exception cref="Exceptions.MissingConfigurationException"></exception>
        /// <exception cref="Exceptions.NoKeyEntityInNavigationManyException"></exception>
        TEntity MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener operationListener)
            where TEntity : class;

        /// <summary>
        /// Perform a merge on a single entity with a custom merge configuration.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="existingEntity"></param>
        /// <param name="newEntity"></param>
        /// <param name="mergeSingleConfigurationAction"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.DuplicateKeysException"></exception>
        /// <exception cref="Exceptions.MissingConfigurationException"></exception>
        /// <exception cref="Exceptions.NoKeyEntityInNavigationManyException"></exception>
        TEntity MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity, Action<IMergeSingleConfiguration> mergeSingleConfigurationAction)
            where TEntity : class;

        /// <summary>
        /// Perform a merge on a single entity with an operation listener and a custom merge configuration.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="existingEntity"></param>
        /// <param name="newEntity"></param>
        /// <param name="operationListener"></param>
        /// <param name="mergeSingleConfigurationAction"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.DuplicateKeysException"></exception>
        /// <exception cref="Exceptions.MissingConfigurationException"></exception>
        /// <exception cref="Exceptions.NoKeyEntityInNavigationManyException"></exception>
        TEntity MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener operationListener, Action<IMergeSingleConfiguration> mergeSingleConfigurationAction)
            where TEntity : class;

        /// <summary>
        ///  Perform a merge on multiple entities.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="existingEntities"></param>
        /// <param name="newEntities"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.DuplicateKeysException"></exception>
        /// <exception cref="Exceptions.MissingConfigurationException"></exception>
        /// <exception cref="Exceptions.NoKeyEntityInNavigationManyException"></exception>
        IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
            where TEntity : class;

        /// <summary>
        /// Perform a merge on multiple entities with an operation listener.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="existingEntities"></param>
        /// <param name="newEntities"></param>
        /// <param name="operationListener"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.DuplicateKeysException"></exception>
        /// <exception cref="Exceptions.MissingConfigurationException"></exception>
        /// <exception cref="Exceptions.NoKeyEntityInNavigationManyException"></exception>
        IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener operationListener)
            where TEntity : class;

        /// <summary>
        /// Perform a merge on multiple entities with a custom merge configuration.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="existingEntities"></param>
        /// <param name="newEntities"></param>
        /// <param name="mergeManyConfigurationAction"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.DuplicateKeysException"></exception>
        /// <exception cref="Exceptions.MissingConfigurationException"></exception>
        /// <exception cref="Exceptions.NoKeyEntityInNavigationManyException"></exception>
        IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, Action<IMergeManyConfiguration> mergeManyConfigurationAction)
            where TEntity : class;

        /// <summary>
        /// Perform a merge on multiple entities with an operation listener and a custom merge configuration.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="existingEntities"></param>
        /// <param name="newEntities"></param>
        /// <param name="operationListener"></param>
        /// <param name="mergeManyConfigurationAction"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.DuplicateKeysException"></exception>
        /// <exception cref="Exceptions.MissingConfigurationException"></exception>
        /// <exception cref="Exceptions.NoKeyEntityInNavigationManyException"></exception>
        IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener operationListener, Action<IMergeManyConfiguration> mergeManyConfigurationAction)
            where TEntity : class;

        /// <summary>
        /// Compare a single entity using an operation listener to retrieve differences.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="existingEntity"></param>
        /// <param name="newEntity"></param>
        /// <param name="operationListener"></param>
        /// <exception cref="Exceptions.DuplicateKeysException"></exception>
        /// <exception cref="Exceptions.MissingConfigurationException"></exception>
        /// <exception cref="Exceptions.NoKeyEntityInNavigationManyException"></exception>
        void CompareSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener operationListener)
            where TEntity : class;

        /// <summary>
        /// Compare a single entity using an operation listener to retrieve differences and a custom compare configuration.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="existingEntity"></param>
        /// <param name="newEntity"></param>
        /// <param name="operationListener"></param>
        /// <param name="diffSingleConfigurationAction"></param>
        /// <exception cref="Exceptions.DuplicateKeysException"></exception>
        /// <exception cref="Exceptions.MissingConfigurationException"></exception>
        /// <exception cref="Exceptions.NoKeyEntityInNavigationManyException"></exception>
        void CompareSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener operationListener, Action<ICompareSingleConfiguration> diffSingleConfigurationAction)
            where TEntity : class;

        /// <summary>
        /// Compare multiple entities using an operation listener to retrieve differences.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="existingEntities"></param>
        /// <param name="newEntities"></param>
        /// <param name="operationListener"></param>
        /// <exception cref="Exceptions.DuplicateKeysException"></exception>
        /// <exception cref="Exceptions.MissingConfigurationException"></exception>
        /// <exception cref="Exceptions.NoKeyEntityInNavigationManyException"></exception>
        void CompareMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener operationListener)
            where TEntity : class;

        /// <summary>
        /// Compare multiple entities using an operation listener to retrieve differences and a custom compare configuration.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="existingEntities"></param>
        /// <param name="newEntities"></param>
        /// <param name="operationListener"></param>
        /// <param name="diffManyConfigurationAction"></param>
        /// <exception cref="Exceptions.DuplicateKeysException"></exception>
        /// <exception cref="Exceptions.MissingConfigurationException"></exception>
        /// <exception cref="Exceptions.NoKeyEntityInNavigationManyException"></exception>
        void CompareMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener operationListener, Action<ICompareManyConfiguration> diffManyConfigurationAction)
            where TEntity : class;
    }
}