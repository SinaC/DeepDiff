using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.Operations;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("DeepDiff.UnitTest")]
[assembly: InternalsVisibleTo("DeepDiff.PerformanceTest")]

namespace DeepDiff
{
    internal sealed class DeepDiff : IDeepDiff
    {
        private DeepDiffConfiguration Configuration { get; }

        internal DeepDiff(DeepDiffConfiguration configuration)
        {
            Configuration = configuration;
        }

        public DiffSingleResult<TEntity> DiffSingle<TEntity>(TEntity existingEntity, TEntity newEntity)
            where TEntity : class
            => DiffSingle(existingEntity, newEntity, null);

        public DiffSingleResult<TEntity> DiffSingle<TEntity>(TEntity existingEntity, TEntity newEntity, Action<IDiffSingleConfiguration> diffSingleConfigurationAction)
            where TEntity : class
        {
            if (!Configuration.EntityConfigurationByTypes.TryGetValue(typeof(TEntity), out var entityConfiguration))
                throw new MissingConfigurationException(typeof(TEntity));

            var diffSingleConfiguration = new DiffSingleConfiguration();
            diffSingleConfigurationAction?.Invoke(diffSingleConfiguration);

            var engine = new DeepDiffEngine(Configuration.EntityConfigurationByTypes, diffSingleConfiguration.Configuration);
            var diffOperations = new List<DiffOperationBase>();
            var diffEntity = engine.InternalDiffSingle(entityConfiguration, existingEntity, newEntity, diffOperations);
            return new DiffSingleResult<TEntity> 
            {
                Entity = diffSingleConfiguration.Configuration.GenerateOperationsOnly 
                    ? default 
                    : (TEntity)diffEntity,
                Operations = diffOperations
            };
        }

        public DiffManyResult<TEntity> DiffMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
            where TEntity : class
            => DiffMany(existingEntities, newEntities, null);

        public DiffManyResult<TEntity> DiffMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, Action<IDiffManyConfiguration> diffManyConfigurationAction)
            where TEntity : class
        {
            if (!Configuration.EntityConfigurationByTypes.TryGetValue(typeof(TEntity), out var entityConfiguration))
                throw new MissingConfigurationException(typeof(TEntity));

            var diffManyConfiguration = new DiffManyConfiguration();
            diffManyConfigurationAction?.Invoke(diffManyConfiguration);

            var engine = new DeepDiffEngine(Configuration.EntityConfigurationByTypes, diffManyConfiguration.Configuration);
            var diffOperations = new List<DiffOperationBase>();
            var diffEntities = engine.InternalDiffMany(entityConfiguration, existingEntities, newEntities, diffOperations);
            return new DiffManyResult<TEntity>
            {
                Entities = diffManyConfiguration.Configuration.GenerateOperationsOnly
                    ? default
                    : diffEntities.Cast<TEntity>(),
                Operations = diffOperations
            };
        }
    }
}