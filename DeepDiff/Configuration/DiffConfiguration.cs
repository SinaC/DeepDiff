using DeepDiff.Exceptions;
using DeepDiff.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Configuration
{
    public sealed class DiffConfiguration : IDiffConfiguration
    {
        internal Dictionary<Type, DiffEntityConfiguration> DiffEntityConfigurationByTypes { get; private set; } = new Dictionary<Type, DiffEntityConfiguration>();

        public IDiffEntityConfiguration<TEntity> Entity<TEntity>()
            where TEntity : class
        {
            var entityType = typeof(TEntity);
            if (DiffEntityConfigurationByTypes.ContainsKey(entityType))
                throw new DuplicateDiffEntityConfigurationException(entityType);

            var diffEntityConfiguration = new DiffEntityConfiguration(entityType);
            DiffEntityConfigurationByTypes.Add(entityType, diffEntityConfiguration);

            return new DiffEntityConfiguration<TEntity>(diffEntityConfiguration);
        }

        public IDiffConfiguration AddProfile<TProfile>()
            where TProfile : DiffProfile
        {
            DiffProfile diffProfileInstance;
            try
            {
                diffProfileInstance = (DiffProfile)Activator.CreateInstance(typeof(TProfile))!;
            }
            catch (TargetInvocationException ex) when (ex.InnerException is DuplicateDiffEntityConfigurationException)
            {
                throw ex.InnerException;
            }
            foreach (var typeAndDiffEntityConfiguration in diffProfileInstance.DiffEntityConfigurations)
            {
                if (DiffEntityConfigurationByTypes.ContainsKey(typeAndDiffEntityConfiguration.Key))
                    throw new DuplicateDiffEntityConfigurationException(typeAndDiffEntityConfiguration.Key);
                DiffEntityConfigurationByTypes.Add(typeAndDiffEntityConfiguration.Key, typeAndDiffEntityConfiguration.Value);
            }
            return this;
        }

        public IDiffConfiguration AddProfiles(params Assembly[] assembliesToScan)
        {
            if (assembliesToScan != null && assembliesToScan.Length > 0)
            {
                foreach (var assembly in assembliesToScan)
                {
                    var diffProfileType = typeof(DiffProfile);
                    foreach (var derivedDiffProfileType in assembly.GetTypes().Where(x => x != diffProfileType && diffProfileType.IsAssignableFrom(x)))
                    {
                        var diffProfileInstance = (DiffProfile)Activator.CreateInstance(derivedDiffProfileType)!;
                        foreach (var typeAndDiffEntityConfiguration in diffProfileInstance.DiffEntityConfigurations)
                            DiffEntityConfigurationByTypes.Add(typeAndDiffEntityConfiguration.Key, typeAndDiffEntityConfiguration.Value);
                    }
                }
            }
            return this;
        }

        public IDeepDiff CreateDeepDiff()
        {
            ValidateConfiguration();

            return new DeepDiff(this);
        }

        public void ValidateConfiguration()
        {
            var exceptions = new List<Exception>();

            var validators = new ValidatorBase[]
            {
                new KeyValidator(),
                new ValuesValidator(),
                new NavigationManyValidator(),
                new NavigationOneValidator(),
                new UpdateValidator(),
                new InsertValidator(),
                new DeleteValidator()
            };

            foreach (var (type, diffEntityConfiguration) in DiffEntityConfigurationByTypes)
            {
                foreach (var validator in validators)
                {
                    var validationExceptions = validator.Validate(type, diffEntityConfiguration, DiffEntityConfigurationByTypes);
                    exceptions.AddRange(validationExceptions);
                }
            }
            if (exceptions.Count == 1)
                throw exceptions.Single();
            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }
    }
}