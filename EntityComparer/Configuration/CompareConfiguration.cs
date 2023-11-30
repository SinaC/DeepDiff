using EntityComparer.Exceptions;
using EntityComparer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntityComparer.Configuration
{
    public sealed class CompareConfiguration : ICompareConfiguration
    {
        internal IDictionary<Type, CompareEntityConfiguration> CompareEntityConfigurationByTypes { get; private set; } = new Dictionary<Type, CompareEntityConfiguration>();
        internal bool UseHashtable { get; private set; } = true;
        internal int HashtableThreshold { get; private set; } = 15;

        public ICompareEntityConfiguration<TEntity> Entity<TEntity>()
            where TEntity : class
        {
            var compareEntityConfiguration = new CompareEntityConfiguration(typeof(TEntity));
            CompareEntityConfigurationByTypes.Add(typeof(TEntity), compareEntityConfiguration);

            return new CompareEntityConfiguration<TEntity>(compareEntityConfiguration);
        }

        public ICompareConfiguration AddProfile<TProfile>()
            where TProfile : CompareProfile
        {
            var compareProfileInstance = (CompareProfile)Activator.CreateInstance(typeof(TProfile))!;
            foreach (var typeAndCompareEntityConfiguration in compareProfileInstance.CompareEntityConfigurations)
                CompareEntityConfigurationByTypes.Add(typeAndCompareEntityConfiguration.Key, typeAndCompareEntityConfiguration.Value);
            return this;
        }

        public ICompareConfiguration AddProfiles(params Assembly[] assembliesToScan)
        {
            if (assembliesToScan != null && assembliesToScan.Length > 0)
            {
                foreach (var assembly in assembliesToScan)
                {
                    var compareProfileType = typeof(CompareProfile);
                    foreach (var derivedCompareProfileType in assembly.GetTypes().Where(x => x != compareProfileType && compareProfileType.IsAssignableFrom(x)))
                    {
                        var compareProfileInstance = (CompareProfile)Activator.CreateInstance(derivedCompareProfileType)!;
                        foreach (var typeAndCompareEntityConfiguration in compareProfileInstance.CompareEntityConfigurations)
                            CompareEntityConfigurationByTypes.Add(typeAndCompareEntityConfiguration.Key, typeAndCompareEntityConfiguration.Value);
                    }
                }
            }
            return this;
        }

        public ICompareConfiguration DisableHashtable()
        {
            UseHashtable = false;
            return this;
        }

        public ICompareConfiguration SetHashtableThreshold(int threshold)
        {
            HashtableThreshold = threshold;
            return this;
        }

        public IEntityComparer CreateComparer()
        {
            ValidateConfiguration();

            return new EntityComparer(this);
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
            foreach (var compareEntityConfigurationByType in CompareEntityConfigurationByTypes)
            {
                var type = compareEntityConfigurationByType.Key;
                var compareEntityConfiguration = compareEntityConfigurationByType.Value;

                ValidateKeyConfiguration(type, compareEntityConfiguration, exceptions);
                ValidateValuesConfiguration(type, compareEntityConfiguration, exceptions);
                ValidateAdditionalValuesToCopy(type, compareEntityConfiguration, exceptions);
                ValidateNavigationManyConfiguration(type, compareEntityConfiguration, CompareEntityConfigurationByTypes, exceptions);
                ValidateNavigationOneConfiguration(type, compareEntityConfiguration, CompareEntityConfigurationByTypes, exceptions);
                ValidateMarkAsConfiguration(type, compareEntityConfiguration, exceptions);
            }
            if (exceptions.Count == 1)
                throw exceptions.Single();
            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        private static void ValidateKeyConfiguration(Type entityType, CompareEntityConfiguration compareEntityConfiguration, List<Exception> exceptions)
        {
            var configuration = compareEntityConfiguration.KeyConfiguration;
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

        private static void ValidateValuesConfiguration(Type entityType, CompareEntityConfiguration compareEntityConfiguration, List<Exception> exceptions)
        {
            var configuration = compareEntityConfiguration.ValuesConfiguration;
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
                    if (compareEntityConfiguration.KeyConfiguration?.KeyProperties != null)
                    {
                        var alreadyDefinedInKey = configuration.ValuesProperties.Intersect(compareEntityConfiguration.KeyConfiguration.KeyProperties).ToArray();
                        if (alreadyDefinedInKey.Length > 0)
                            exceptions.Add(new AlreadyDefinedPropertyException(entityType, NameOf<ValuesConfiguration>(), NameOf<KeyConfiguration>(), alreadyDefinedInKey.Select(x => x.Name)));
                    }
                }
            }
        }

        private static void ValidateAdditionalValuesToCopy(Type entityType, CompareEntityConfiguration compareEntityConfiguration, List<Exception> exceptions)
        {
            var configuration = compareEntityConfiguration.AdditionalValuesToCopyConfiguration;
            if (configuration != null)
            {
                // cannot be empty
                if (configuration.AdditionalValuesToCopyProperties == null || configuration.AdditionalValuesToCopyProperties.Count == 0)
                    exceptions.Add(new EmptyConfigurationException(entityType, NameOf<AdditionalValuesToCopyConfiguration>()));
                else
                {
                    // cannot contain duplicates
                    var duplicates = configuration.AdditionalValuesToCopyProperties.FindDuplicate().ToArray();
                    if (duplicates.Length > 0)
                        exceptions.Add(new DuplicatePropertyConfigurationException(entityType, NameOf<AdditionalValuesToCopyConfiguration>(), duplicates.Select(x => x.Name)));
                    // cannot be defined in keys
                    if (compareEntityConfiguration.KeyConfiguration?.KeyProperties != null)
                    {
                        var alreadyDefinedInKey = configuration.AdditionalValuesToCopyProperties.Intersect(compareEntityConfiguration.KeyConfiguration.KeyProperties).ToArray();
                        if (alreadyDefinedInKey.Length > 0)
                            exceptions.Add(new AlreadyDefinedPropertyException(entityType, NameOf<AdditionalValuesToCopyConfiguration>(), NameOf<KeyConfiguration>(), alreadyDefinedInKey.Select(x => x.Name)));
                    }
                    // cannot be found in values
                    if (compareEntityConfiguration.ValuesConfiguration?.ValuesProperties != null)
                    {
                        var alreadyDefinedInKey = configuration.AdditionalValuesToCopyProperties.Intersect(compareEntityConfiguration.ValuesConfiguration.ValuesProperties).ToArray();
                        if (alreadyDefinedInKey.Length > 0)
                            exceptions.Add(new AlreadyDefinedPropertyException(entityType, NameOf<AdditionalValuesToCopyConfiguration>(), NameOf<ValuesConfiguration>(), alreadyDefinedInKey.Select(x => x.Name)));
                    }
                }
            }
        }

        private static void ValidateNavigationManyConfiguration(Type entityType, CompareEntityConfiguration compareEntityConfiguration, IDictionary<Type, CompareEntityConfiguration> compareEntityConfigurationByTypes, List<Exception> exceptions)
        {
            if (compareEntityConfiguration.NavigationManyConfigurations != null)
            {
                foreach (var configuration in compareEntityConfiguration.NavigationManyConfigurations)
                {
                    // check if navigation child type is found in configuration
                    if (!compareEntityConfigurationByTypes.ContainsKey(configuration.NavigationManyChildType))
                        exceptions.Add(new MissingNavigationManyChildConfigurationException(entityType, configuration.NavigationManyChildType));
                }
            }
        }

        private static void ValidateNavigationOneConfiguration(Type entityType, CompareEntityConfiguration compareEntityConfiguration, IDictionary<Type, CompareEntityConfiguration> compareEntityConfigurationByTypes, List<Exception> exceptions)
        {
            if (compareEntityConfiguration.NavigationOneConfigurations != null)
            {
                foreach (var configuration in compareEntityConfiguration.NavigationOneConfigurations)
                {
                    // check if navigation one property is not a collection
                    if (configuration.NavigationOneProperty.IsEnumerable())
                        exceptions.Add(new InvalidNavigationOneChildTypeConfigurationException(entityType, configuration.NavigationOneProperty.Name));
                    else
                    {
                        // check if navigation child type is found in configuration
                        if (!compareEntityConfigurationByTypes.ContainsKey(configuration.NavigationOneChildType))
                            exceptions.Add(new MissingNavigationOneChildConfigurationException(entityType, configuration.NavigationOneChildType));
                    }
                }
            }
        }

        private static void ValidateMarkAsConfiguration(Type entityType, CompareEntityConfiguration compareEntityConfiguration, List<Exception> exceptions)
        {
            var configuration = compareEntityConfiguration.MarkAsByOperation;
            ValidateMarkAsConfiguration(entityType, configuration, CompareEntityOperation.Insert, exceptions);
            ValidateMarkAsConfiguration(entityType, configuration, CompareEntityOperation.Update, exceptions);
            ValidateMarkAsConfiguration(entityType, configuration, CompareEntityOperation.Delete, exceptions);
        }

        private static void ValidateMarkAsConfiguration(Type entityType, IDictionary<CompareEntityOperation, MarkAsConfiguration> markAsConfiguration, CompareEntityOperation compareEntityOperation, List<Exception> exceptions)
        {
            var found = markAsConfiguration.ContainsKey(compareEntityOperation);
            if (!found)
                exceptions.Add(new MissingMarkAsConfigurationException(entityType, compareEntityOperation));
        }

        internal static string NameOf<T>()
            => typeof(T).Name.Replace("Configuration", string.Empty);

    }
}