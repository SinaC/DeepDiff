using DeepDiff.Exceptions;
using DeepDiff.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace DeepDiff.Configuration
{
    public sealed class DiffConfiguration : IDiffConfiguration
    {
        internal IDictionary<Type, DiffEntityConfiguration> DiffEntityConfigurationByTypes { get; private set; } = new Dictionary<Type, DiffEntityConfiguration>();
        internal bool UseHashtable { get; private set; } = true;
        internal int HashtableThreshold { get; private set; } = 15;

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

        public IDiffConfiguration DisableHashtable()
        {
            UseHashtable = false;
            return this;
        }

        public IDiffConfiguration SetHashtableThreshold(int threshold)
        {
            HashtableThreshold = threshold;
            return this;
        }

        public IDeepDiff CreateDeepDiff()
        {
            ValidateConfiguration();

            return new DeepDiff(this);
        }

        private void ValidateConfiguration()
        {
            var exceptions = new List<Exception>();

            // KeyConfiguration: cannot be null/empty and every property must be different
            // ValuesConfiguration: if not null, cannot be empty, every property must be different and cannot be found in key configuration
            // AdditionalValuesToCopyConfiguration: if not null, cannot be empty, every property must be different and cannot be found in values nor key configuration
            // NavigationManyConfiguration: every NavigationManyChildType must exist in configuration
            // NavigationOneConfiguration: every NavigationOneProperty cannot be a collection and must exist in configuration
            // MarkAsConfiguration: cannot be null
            foreach (var diffEntityConfigurationByType in DiffEntityConfigurationByTypes)
            {
                var type = diffEntityConfigurationByType.Key;
                var diffEntityConfiguration = diffEntityConfigurationByType.Value;

                ValidateKeyConfiguration(type, diffEntityConfiguration, exceptions);
                ValidateValuesConfiguration(type, diffEntityConfiguration, exceptions);
                ValidateNavigationManyConfiguration(type, diffEntityConfiguration, DiffEntityConfigurationByTypes, exceptions);
                ValidateNavigationOneConfiguration(type, diffEntityConfiguration, DiffEntityConfigurationByTypes, exceptions);
                ValidateOperationConfiguration(type, diffEntityConfiguration, exceptions);
            }
            if (exceptions.Count == 1)
                throw exceptions.Single();
            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        private static void ValidateKeyConfiguration(Type entityType, DiffEntityConfiguration diffEntityConfiguration, List<Exception> exceptions)
        {
            var configuration = diffEntityConfiguration.KeyConfiguration;
            // cannot be null
            if (configuration == null)
                exceptions.Add(new MissingKeyConfigurationException(entityType));
            else
            {
                // cannot be empty
                if (configuration.KeyProperties == null || configuration.KeyProperties.Count == 0)
                    exceptions.Add(new EmptyConfigurationException(entityType, NameOf<KeyConfiguration>()));
                else
                {
                    // cannot contain duplicates
                    var duplicates = configuration.KeyProperties.FindDuplicate().ToArray();
                    if (duplicates.Length > 0)
                        exceptions.Add(new DuplicatePropertyConfigurationException(entityType, NameOf<KeyConfiguration>(), duplicates.Select(x => x.Name)));
                }
            }
        }

        private static void ValidateValuesConfiguration(Type entityType, DiffEntityConfiguration diffEntityConfiguration, List<Exception> exceptions)
        {
            var configuration = diffEntityConfiguration.ValuesConfiguration;
            if (configuration != null)
            {
                // cannot be empty
                if (configuration.ValuesProperties == null || configuration.ValuesProperties.Count == 0)
                    exceptions.Add(new EmptyConfigurationException(entityType, NameOf<ValuesConfiguration>()));
                else
                {
                    // cannot contain duplicates
                    var duplicates = configuration.ValuesProperties.FindDuplicate().ToArray();
                    if (duplicates.Length > 0)
                        exceptions.Add(new DuplicatePropertyConfigurationException(entityType, NameOf<ValuesConfiguration>(), duplicates.Select(x => x.Name)));
                    // cannot be defined in keys
                    if (diffEntityConfiguration.KeyConfiguration?.KeyProperties != null)
                    {
                        var alreadyDefinedInKey = configuration.ValuesProperties.Intersect(diffEntityConfiguration.KeyConfiguration.KeyProperties).ToArray();
                        if (alreadyDefinedInKey.Length > 0)
                            exceptions.Add(new AlreadyDefinedPropertyException(entityType, NameOf<ValuesConfiguration>(), NameOf<KeyConfiguration>(), alreadyDefinedInKey.Select(x => x.Name)));
                    }
                }
            }
        }

        private static void ValidateNavigationManyConfiguration(Type entityType, DiffEntityConfiguration diffEntityConfiguration, IDictionary<Type, DiffEntityConfiguration> diffEntityConfigurationByTypes, List<Exception> exceptions)
        {
            if (diffEntityConfiguration.NavigationManyConfigurations != null)
            {
                foreach (var configuration in diffEntityConfiguration.NavigationManyConfigurations)
                {
                    // check if navigation child type is found in configuration
                    if (!diffEntityConfigurationByTypes.ContainsKey(configuration.NavigationManyChildType))
                        exceptions.Add(new MissingNavigationManyChildConfigurationException(entityType, configuration.NavigationManyChildType));
                }
            }
        }

        private static void ValidateNavigationOneConfiguration(Type entityType, DiffEntityConfiguration diffEntityConfiguration, IDictionary<Type, DiffEntityConfiguration> diffEntityConfigurationByTypes, List<Exception> exceptions)
        {
            if (diffEntityConfiguration.NavigationOneConfigurations != null)
            {
                foreach (var configuration in diffEntityConfiguration.NavigationOneConfigurations)
                {
                    // check if navigation one property is not a collection
                    if (configuration.NavigationOneProperty.IsEnumerable())
                        exceptions.Add(new InvalidNavigationOneChildTypeConfigurationException(entityType, configuration.NavigationOneProperty.Name));
                    else
                    {
                        // check if navigation child type is found in configuration
                        if (!diffEntityConfigurationByTypes.ContainsKey(configuration.NavigationOneChildType))
                            exceptions.Add(new MissingNavigationOneChildConfigurationException(entityType, configuration.NavigationOneChildType));
                    }
                }
            }
        }

        private static void ValidateOperationConfiguration(Type entityType, DiffEntityConfiguration diffEntityConfiguration, List<Exception> exceptions)
        {
            ValidateUpdateConfiguration(entityType, diffEntityConfiguration, exceptions);
            ValidateInsertConfiguration(entityType, diffEntityConfiguration, exceptions);
            ValidateDeleteConfiguration(entityType, diffEntityConfiguration, exceptions);
        }

        private static void ValidateUpdateConfiguration(Type entityType, DiffEntityConfiguration diffEntityConfiguration, List<Exception> exceptions)
        {
            var updateConfiguration = diffEntityConfiguration.UpdateConfiguration;
            if (diffEntityConfiguration.UpdateConfiguration == null)
                exceptions.Add(new MissingOnUpdateConfigurationException(entityType));
            else
            {
                // set value
                if (updateConfiguration.SetValueConfiguration == null)
                    exceptions.Add(new MissingSetValueConfigurationException(entityType, "OnUpdate"));

                // copy values
                var copyValuesConfigurations = updateConfiguration.CopyValuesConfiguration;
                if (copyValuesConfigurations != null)
                {
                    // cannot be empty
                    if (copyValuesConfigurations.CopyValuesProperties == null || copyValuesConfigurations.CopyValuesProperties.Count == 0)
                        exceptions.Add(new EmptyConfigurationException(entityType, NameOf<CopyValuesConfiguration>()));
                    else
                    {
                        // cannot contain duplicates
                        var duplicates = copyValuesConfigurations.CopyValuesProperties.FindDuplicate().ToArray();
                        if (duplicates.Length > 0)
                            exceptions.Add(new DuplicatePropertyConfigurationException(entityType, NameOf<CopyValuesConfiguration>(), duplicates.Select(x => x.Name)));
                        // cannot be defined in keys
                        if (diffEntityConfiguration.KeyConfiguration?.KeyProperties != null)
                        {
                            var alreadyDefinedInKey = copyValuesConfigurations.CopyValuesProperties.Intersect(diffEntityConfiguration.KeyConfiguration.KeyProperties).ToArray();
                            if (alreadyDefinedInKey.Length > 0)
                                exceptions.Add(new AlreadyDefinedPropertyException(entityType, NameOf<CopyValuesConfiguration>(), NameOf<KeyConfiguration>(), alreadyDefinedInKey.Select(x => x.Name)));
                        }
                        // cannot be found in values
                        if (diffEntityConfiguration.ValuesConfiguration?.ValuesProperties != null)
                        {
                            var alreadyDefinedInKey = copyValuesConfigurations.CopyValuesProperties.Intersect(diffEntityConfiguration.ValuesConfiguration.ValuesProperties).ToArray();
                            if (alreadyDefinedInKey.Length > 0)
                                exceptions.Add(new AlreadyDefinedPropertyException(entityType, NameOf<CopyValuesConfiguration>(), NameOf<ValuesConfiguration>(), alreadyDefinedInKey.Select(x => x.Name)));
                        }
                    }
                }
            }
        }

        private static void ValidateInsertConfiguration(Type entityType, DiffEntityConfiguration diffEntityConfiguration, List<Exception> exceptions)
        {
            var insertConfiguration = diffEntityConfiguration.InsertConfiguration;
            if (insertConfiguration == null)
                exceptions.Add(new MissingOnInsertConfigurationException(entityType));
            else
            {
                if (insertConfiguration.SetValueConfiguration == null)
                    exceptions.Add(new MissingSetValueConfigurationException(entityType, "OnInsert"));
            }
        }

        private static void ValidateDeleteConfiguration(Type entityType, DiffEntityConfiguration diffEntityConfiguration, List<Exception> exceptions)
        {
            var deleteConfiguration = diffEntityConfiguration.DeleteConfiguration;
            if (deleteConfiguration == null)
                exceptions.Add(new MissingOnDeleteConfigurationException(entityType));
            else
            {
                if (deleteConfiguration.SetValueConfiguration == null)
                    exceptions.Add(new MissingSetValueConfigurationException(entityType, "OnDelete"));
            }
        }

        internal static string NameOf<T>()
            => typeof(T).Name.Replace("Configuration", string.Empty);

    }
}