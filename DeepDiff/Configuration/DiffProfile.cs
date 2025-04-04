using DeepDiff.Exceptions;
using DeepDiff.Internal.Configuration;
using System;
using System.Collections.Generic;

namespace DeepDiff.Configuration
{
    /// <summary>
    /// Base class for defining a profile for entity configurations.
    /// </summary>
    public abstract class DiffProfile
    {
        internal Dictionary<Type, EntityConfiguration> EntityConfigurations { get; private set; } = new Dictionary<Type, EntityConfiguration>();

        /// <summary>
        /// Creates a new instance of <see cref="IEntityConfiguration{TEntity}"/> for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="DuplicateEntityConfigurationException"></exception>
        protected IEntityConfiguration<TEntity> CreateConfiguration<TEntity>()
            where TEntity : class
        {
            var entityType = typeof(TEntity);

            if (entityType.IsAbstract)
                throw new AbstractEntityConfigurationException(entityType);

            if (EntityConfigurations.ContainsKey(entityType))
                throw new DuplicateEntityConfigurationException(entityType);

            var entityConfiguration = new EntityConfiguration(entityType);
            EntityConfigurations.Add(entityType, entityConfiguration);

            return new EntityConfiguration<TEntity>(entityConfiguration);
        }
    }
}
