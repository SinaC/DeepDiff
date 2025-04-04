using DeepDiff.Exceptions;
using DeepDiff.Internal.Configuration;
using DeepDiff.Internal.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Configuration
{
    public sealed class DeepDiffConfiguration : IDeepDiffConfiguration
    {
        internal Dictionary<Type, EntityConfiguration> EntityConfigurationByTypes { get; } = new Dictionary<Type, EntityConfiguration>();

        /// <summary>
        /// Creates a new instance of <see cref="IEntityConfiguration{TEntity}"/> for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="AbstractEntityConfigurationException"/>
        /// <exception cref="DuplicateEntityConfigurationException"/>
        public IEntityConfiguration<TEntity> Entity<TEntity>()
            where TEntity : class
        {
            var entityType = typeof(TEntity);

            if (entityType.IsAbstract)
                throw new AbstractEntityConfigurationException(entityType);

            if (EntityConfigurationByTypes.ContainsKey(entityType))
                throw new DuplicateEntityConfigurationException(entityType);

            var entityConfiguration = new EntityConfiguration(entityType);
            EntityConfigurationByTypes.Add(entityType, entityConfiguration);

            return new EntityConfiguration<TEntity>(entityConfiguration);
        }

        /// <summary>
        /// Adds a profile to the configuration.
        /// </summary>
        /// <typeparam name="TProfile"></typeparam>
        /// <returns></returns>
        /// <exception cref="AbstractEntityConfigurationException"/>
        /// <exception cref="DuplicateEntityConfigurationException"/>
        public IDeepDiffConfiguration AddProfile<TProfile>()
            where TProfile : DiffProfile
        {
            var diffProfileInstance = CreateProfileInstance(typeof(TProfile));
            AddProfileFromInstance(diffProfileInstance);
            return this;
        }

        /// <summary>
        /// Adds a profile to the configuration.
        /// </summary>
        /// <param name="diffProfile"></param>
        /// <returns></returns>
        /// <exception cref="AbstractEntityConfigurationException"/>
        /// <exception cref="DuplicateEntityConfigurationException"/>
        public IDeepDiffConfiguration AddProfile(DiffProfile diffProfile)
        {
            AddProfileFromInstance(diffProfile);
            return this;
        }

        /// <summary>
        /// Adds profiles from the specified assemblies to the configuration.
        /// </summary>
        /// <param name="assembliesToScan"></param>
        /// <returns></returns>
        /// <exception cref="AbstractEntityConfigurationException"/>
        /// <exception cref="DuplicateEntityConfigurationException"/>
        public IDeepDiffConfiguration AddProfiles(params Assembly[] assembliesToScan)
        {
            if (assembliesToScan != null && assembliesToScan.Length > 0)
            {
                foreach (var assembly in assembliesToScan)
                {
                    var diffProfileType = typeof(DiffProfile);
                    foreach (var derivedDiffProfileType in assembly.GetTypes().Where(x => x != diffProfileType && diffProfileType.IsAssignableFrom(x)))
                    {
                        var diffProfileInstance = CreateProfileInstance(derivedDiffProfileType);
                        AddProfileFromInstance(diffProfileInstance);
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Creates a new instance of <see cref="IDeepDiff"/> with the current configuration.
        /// </summary>
        /// <returns></returns>
        /// /// <exception cref="AggregateException"/>
        /// <exception cref="AlreadyDefinedPropertyException"/>
        /// <exception cref="DuplicatePropertyConfigurationException"/>
        /// <exception cref="EmptyConfigurationException"/>
        /// <exception cref="MissingKeyConfigurationException"/>
        /// <exception cref="MissingNavigationOneChildConfigurationException"/>
        /// <exception cref="MissingNavigationManyAbstractChildConfigurationException"/>
        /// <exception cref="MissingNavigationManyChildConfigurationException"/>
        /// <exception cref="NoKeyAndHasKeyConfigurationException"/>
        /// <exception cref="NoKeyButFoundInNavigationManyConfigurationException"/>
        /// <exception cref="InvalidNavigationOneChildTypeConfigurationException"/>
        public IDeepDiff CreateDeepDiff()
        {
            ValidateConfiguration();
            CreateComparers();

            return new Internal.DeepDiff(this);
        }

        /// <summary>
        /// Validates the configuration. Throw an exception if the configuration is invalid.
        /// </summary>
        /// <exception cref="AggregateException"/>
        /// <exception cref="AlreadyDefinedPropertyException"/>
        /// <exception cref="DuplicatePropertyConfigurationException"/>
        /// <exception cref="EmptyConfigurationException"/>
        /// <exception cref="MissingKeyConfigurationException"/>
        /// <exception cref="MissingNavigationOneChildConfigurationException"/>
        /// <exception cref="MissingNavigationManyAbstractChildConfigurationException"/>
        /// <exception cref="MissingNavigationManyChildConfigurationException"/>
        /// <exception cref="NoKeyAndHasKeyConfigurationException"/>
        /// <exception cref="NoKeyButFoundInNavigationManyConfigurationException"/>
        /// <exception cref="InvalidNavigationOneChildTypeConfigurationException"/>
        public void ValidateConfiguration()
        {
            var validators = new ValidatorBase[]
            {
                new NoKeyValidator(),
                new KeyValidator(),
                new ValuesValidator(),
                new NavigationManyValidator(),
                new NavigationOneValidator(),
                new UpdateValidator(),
                new InsertValidator(),
                new DeleteValidator(),
                new ComparerValidator(),
            };

            var exceptions = new List<Exception>();
            foreach (var (type, entityConfiguration) in EntityConfigurationByTypes)
            {
                foreach (var validator in validators)
                {
                    var validationExceptions = validator.Validate(type, entityConfiguration, EntityConfigurationByTypes);
                    exceptions.AddRange(validationExceptions);
                }
            }
            if (exceptions.Count == 1)
                throw exceptions.Single();
            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        /// <summary>
        /// Validates if every properties are referenced in the configuration. Throw an exception if a property is not referenced.
        /// </summary>
        /// <exception cref="AggregateException"/>
        /// <exception cref="PropertyNotReferenceInConfigurationException"/>
        public void ValidateIfEveryPropertiesAreReferenced()
        {
            var validator = new CheckEveryPropertiesAreReferencedValidator();

            var exceptions = new List<Exception>();
            foreach (var (type, entityConfiguration) in EntityConfigurationByTypes)
            {
                var validationExceptions = validator.Validate(type, entityConfiguration, EntityConfigurationByTypes);
                exceptions.AddRange(validationExceptions);
            }
            if (exceptions.Count == 1)
                throw exceptions.Single();
            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        private void CreateComparers()
        {
            foreach (var (_, entityConfiguration) in EntityConfigurationByTypes)
                entityConfiguration.CreateComparers();
        }

        private static DiffProfile CreateProfileInstance(Type diffProfileType)
        {
            DiffProfile diffProfileInstance;
            try
            {
                diffProfileInstance = (DiffProfile)Activator.CreateInstance(diffProfileType)!;
            }
            catch (TargetInvocationException ex) when (ex.InnerException is DuplicateEntityConfigurationException)
            {
                throw ex.InnerException;
            }
            return diffProfileInstance;
        }

        private void AddProfileFromInstance(DiffProfile diffProfile)
        {
            foreach (var typeAndEntityConfiguration in diffProfile.EntityConfigurations)
            {
                var entityType = typeAndEntityConfiguration.Key;

                if (entityType.IsAbstract)
                    throw new AbstractEntityConfigurationException(entityType);

                if (EntityConfigurationByTypes.ContainsKey(entityType))
                    throw new DuplicateEntityConfigurationException(entityType);
                EntityConfigurationByTypes.Add(entityType, typeAndEntityConfiguration.Value);
            }
        }
    }
}