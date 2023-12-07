using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using System;
using System.Collections.Generic;

namespace DeepDiff
{
    internal sealed class DeepDiff : IDeepDiff
    {
        private DiffConfiguration Configuration { get; }

        internal DeepDiff(DiffConfiguration configuration)
        {
            Configuration = configuration;
        }

        public TEntity DiffSingle<TEntity>(TEntity existingEntity, TEntity newEntity)
            where TEntity : class
            => DiffSingle(existingEntity, newEntity, null);

        public TEntity DiffSingle<TEntity>(TEntity existingEntity, TEntity newEntity, Action<IDiffSingleConfiguration> diffSingleConfigurationAction)
            where TEntity : class
        {
            if (!Configuration.DiffEntityConfigurationByTypes.TryGetValue(typeof(TEntity), out var diffEntityConfiguration))
                throw new MissingConfigurationException(typeof(TEntity));

            var diffSingleConfiguration = new DiffSingleConfiguration();
            diffSingleConfigurationAction?.Invoke(diffSingleConfiguration);

            var engine = new DeepDiffEngine(Configuration.DiffEntityConfigurationByTypes, diffSingleConfiguration);
            var diffEntity = engine.InternalDiffSingle(diffEntityConfiguration, existingEntity, newEntity);
            return (TEntity)diffEntity;
        }

        public IEnumerable<TEntity> DiffMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
            where TEntity : class
            => DiffMany(existingEntities, newEntities, null);

        public IEnumerable<TEntity> DiffMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, Action<IDiffManyConfiguration> diffManyConfigurationAction)
            where TEntity : class
        {
            if (!Configuration.DiffEntityConfigurationByTypes.TryGetValue(typeof(TEntity), out var diffEntityConfiguration))
                throw new MissingConfigurationException(typeof(TEntity));

            var diffManyConfiguration = new DiffManyConfiguration();
            diffManyConfigurationAction?.Invoke(diffManyConfiguration);

            var engine = new DeepDiffEngine(Configuration.DiffEntityConfigurationByTypes, diffManyConfiguration);
            var diffEntities = engine.InternalDiffMany(diffEntityConfiguration, existingEntities, newEntities);
            foreach (var diffEntity in diffEntities)
                yield return (TEntity)diffEntity;
        }
    }
}