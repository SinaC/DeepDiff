using DeepDiff.Exceptions;
using DeepDiff.Validators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Configuration
{
    public sealed class DiffConfiguration : IDiffConfiguration
    {
        private IReadOnlyDictionary<Type, IEqualityComparer> TypeSpecificComparers { get;  }

        internal Dictionary<Type, DiffEntityConfiguration> DiffEntityConfigurationByTypes { get; private set; } = new Dictionary<Type, DiffEntityConfiguration>();

        public DiffConfiguration()
        {
        }

        public DiffConfiguration(IReadOnlyDictionary<Type, IEqualityComparer> typeSpecificComparers)
        {
            TypeSpecificComparers = typeSpecificComparers;
        }

        public IDiffEntityConfiguration<TEntity> Entity<TEntity>()
            where TEntity : class
        {
            var entityType = typeof(TEntity);
            if (DiffEntityConfigurationByTypes.ContainsKey(entityType))
                throw new DuplicateDiffEntityConfigurationException(entityType);

            var diffEntityConfiguration = new DiffEntityConfiguration(entityType, TypeSpecificComparers);
            DiffEntityConfigurationByTypes.Add(entityType, diffEntityConfiguration);

            return new DiffEntityConfiguration<TEntity>(diffEntityConfiguration);
        }

        public IDiffConfiguration AddProfile<TProfile>()
            where TProfile : DiffProfile
        {
            var diffProfileInstance = CreateProfileInstance(typeof(TProfile));
            //AddProfileFromInstance(diffProfileInstance);
            return this;
        }

        public IDiffConfiguration AddProfile(DiffProfile diffProfile)
        {
            //AddProfileFromInstance(diffProfile);
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
                        var diffProfileInstance = CreateProfileInstance(derivedDiffProfileType);
                        //AddProfileFromInstance(diffProfileInstance);
                    }
                }
            }
            return this;
        }

        public IDeepDiff CreateDeepDiff()
        {
            ValidateConfiguration();

            CreateComparers();

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

        private void CreateComparers()
        {
            foreach (var (_, diffEntityConfiguration) in DiffEntityConfigurationByTypes)
            {
                diffEntityConfiguration.CreateComparers();
            }
        }

        private DiffProfile CreateProfileInstance(Type diffProfileType)
        {
            DiffProfile diffProfileInstance;
            try
            {
                diffProfileInstance = (DiffProfile)Activator.CreateInstance(diffProfileType, this)!;
            }
            catch (TargetInvocationException ex) when (ex.InnerException is DuplicateDiffEntityConfigurationException)
            {
                throw ex.InnerException;
            }
            return diffProfileInstance;
        }

        //private void AddProfileFromInstance(DiffProfile diffProfile)
        //{
        //    foreach (var typeAndDiffEntityConfiguration in diffProfile.DiffEntityConfigurations)
        //    {
        //        if (DiffEntityConfigurationByTypes.ContainsKey(typeAndDiffEntityConfiguration.Key))
        //            throw new DuplicateDiffEntityConfigurationException(typeAndDiffEntityConfiguration.Key);
        //        DiffEntityConfigurationByTypes.Add(typeAndDiffEntityConfiguration.Key, typeAndDiffEntityConfiguration.Value);
        //    }
        //}
    }
}