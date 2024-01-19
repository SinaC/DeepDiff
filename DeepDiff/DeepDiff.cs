using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.Operations;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("DeepDiff.UnitTest")]

namespace DeepDiff
{
    internal sealed class DeepDiff : IDeepDiff
    {
        private DiffConfiguration Configuration { get; }

        internal DeepDiff(DiffConfiguration configuration)
        {
            Configuration = configuration;
        }

        public DiffSingleResult<TEntity> DiffSingle<TEntity>(TEntity existingEntity, TEntity newEntity)
            where TEntity : class
            => DiffSingle(existingEntity, newEntity, null);

        public DiffSingleResult<TEntity> DiffSingle<TEntity>(TEntity existingEntity, TEntity newEntity, Action<IDiffSingleConfiguration> diffSingleConfigurationAction)
            where TEntity : class
        {
            if (!Configuration.DiffEntityConfigurationByTypes.TryGetValue(typeof(TEntity), out var diffEntityConfiguration))
                throw new MissingConfigurationException(typeof(TEntity));

            var diffSingleConfiguration = new DiffSingleConfiguration();
            diffSingleConfigurationAction?.Invoke(diffSingleConfiguration);

            var engine = new DeepDiffEngine(Configuration.DiffEntityConfigurationByTypes, diffSingleConfiguration);
            var diffOperations = new List<DiffOperationBase>();
            var diffEntity = engine.InternalDiffSingle(diffEntityConfiguration, existingEntity, newEntity, diffOperations);
            return new DiffSingleResult<TEntity> 
            {
                Entity = (TEntity)diffEntity,
                Operations = diffOperations,
                Elapsed = engine.Elapsed
            };
        }

        public DiffManyResult<TEntity> DiffMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
            where TEntity : class
            => DiffMany(existingEntities, newEntities, null);

        public DiffManyResult<TEntity> DiffMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, Action<IDiffManyConfiguration> diffManyConfigurationAction)
            where TEntity : class
        {
            if (!Configuration.DiffEntityConfigurationByTypes.TryGetValue(typeof(TEntity), out var diffEntityConfiguration))
                throw new MissingConfigurationException(typeof(TEntity));

            var diffManyConfiguration = new DiffManyConfiguration();
            diffManyConfigurationAction?.Invoke(diffManyConfiguration);

            var engine = new DeepDiffEngine(Configuration.DiffEntityConfigurationByTypes, diffManyConfiguration);
            var diffOperations = new List<DiffOperationBase>();
            var diffEntities = engine.InternalDiffMany(diffEntityConfiguration, existingEntities, newEntities, diffOperations);
            return new DiffManyResult<TEntity>
            {
                Entities = diffEntities.Cast<TEntity>(),
                Operations = diffOperations,
                Elapsed = engine.Elapsed
            };
        }
    }
}