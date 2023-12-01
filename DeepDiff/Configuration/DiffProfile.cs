using DeepDiff.Exceptions;
using System;
using System.Collections.Generic;

namespace DeepDiff.Configuration
{
    public abstract class DiffProfile
    {
        internal Dictionary<Type, DiffEntityConfiguration> DiffEntityConfigurations { get; private set; } = new Dictionary<Type, DiffEntityConfiguration>();

        protected IDiffEntityConfiguration<TEntity> CreateDiffEntityConfiguration<TEntity>()
            where TEntity : class
        {
            var entityType = typeof(TEntity);
            if (DiffEntityConfigurations.ContainsKey(entityType))
                throw new DuplicateDiffEntityConfigurationException(entityType);

            var diffEntityConfiguration = new DiffEntityConfiguration(entityType);
            DiffEntityConfigurations.Add(entityType, diffEntityConfiguration);

            return new DiffEntityConfiguration<TEntity>(diffEntityConfiguration);
        }
    }
}
