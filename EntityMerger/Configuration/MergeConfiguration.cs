using EntityMerger.Exceptions;
using EntityMerger.Extensions;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;

namespace EntityMerger.Configuration;

public sealed class MergeConfiguration : IMergeConfiguration
{
    internal IDictionary<Type, MergeEntityConfiguration> MergeEntityConfigurationByTypes { get; private set; } = new Dictionary<Type, MergeEntityConfiguration>();
    internal bool UseHashtable { get; private set; } = true;
    internal int HashtableThreshold { get; private set; } = 15;

    public IMergeEntityConfiguration<TEntity> Entity<TEntity>()
        where TEntity : class
    {
        var mergeEntityConfiguration = new MergeEntityConfiguration(typeof(TEntity));
        MergeEntityConfigurationByTypes.Add(typeof(TEntity), mergeEntityConfiguration);

        return new MergeEntityConfiguration<TEntity>(mergeEntityConfiguration);
    }

    public IMergeConfiguration AddProfile<TProfile>()
        where TProfile : MergeProfile
    {
        var mergeProfileInstance = (MergeProfile)Activator.CreateInstance(typeof(TProfile))!;
        foreach (var typeAndMergeEntityConfiguration in mergeProfileInstance.MergeEntityConfigurations)
            MergeEntityConfigurationByTypes.Add(typeAndMergeEntityConfiguration.Key, typeAndMergeEntityConfiguration.Value);
        return this;
    }

    public IMergeConfiguration AddProfiles(params Assembly[] assembliesToScan)
    {
        foreach (var assembly in assembliesToScan)
        {
            var mergeProfileType = typeof(MergeProfile);
            foreach (var derivedMergeProfileType in assembly.GetTypes().Where(x => x != mergeProfileType && mergeProfileType.IsAssignableFrom(x)))
            {
                var mergeProfileInstance = (MergeProfile)Activator.CreateInstance(derivedMergeProfileType)!;
                foreach (var typeAndMergeEntityConfiguration in mergeProfileInstance.MergeEntityConfigurations)
                    MergeEntityConfigurationByTypes.Add(typeAndMergeEntityConfiguration.Key, typeAndMergeEntityConfiguration.Value);
            }
        }
        return this;
    }

    public IMergeConfiguration DisableHashtable()
    {
        UseHashtable = false;
        return this;
    }

    public IMergeConfiguration SetHashtableThreshold(int threshold)
    {
        HashtableThreshold = threshold;
        return this;
    }

    public IMerger CreateMerger()
    {
        ValidateConfiguration();

        return new Merger(this);
    }

    private void ValidateConfiguration()
    {
        var exceptions = new List<Exception>();

        // KeyConfiguration: cannot be null/empty and every property must be different
        // ValuesConfiguration: if not null, cannot be empty, every property must be different and cannot be found in key configuration
        // AdditionalValuesToCopyConfiguration: if not null, cannot be empty, every property must be different and cannot be found in values nor key configuration
        // NavigationManyConfiguration: every NavigationManyChildType must exist in configuration
        // NavigationOneConfiguration: every NavigationOneChildType must exist in configuration   TODO: cannot be a collection
        // MarkAsConfiguration: cannot be null
        foreach (var mergeEntityConfigurationByType in MergeEntityConfigurationByTypes)
        {
            var type = mergeEntityConfigurationByType.Key;
            var mergeEntityConfiguration = mergeEntityConfigurationByType.Value;

            ValidateKeyConfiguration(type, mergeEntityConfiguration, exceptions);
            ValidateValuesConfiguration(type, mergeEntityConfiguration, exceptions);
            ValidateAdditionalValuesToCopy(type, mergeEntityConfiguration, exceptions);
            ValidateNavigationManyConfiguration(type, mergeEntityConfiguration, MergeEntityConfigurationByTypes, exceptions);
            ValidateNavigationOneConfiguration(type, mergeEntityConfiguration, MergeEntityConfigurationByTypes, exceptions);
            ValidateMarkAsConfiguration(type, mergeEntityConfiguration, exceptions);
        }
        if (exceptions.Count > 0)
            throw new AggregateException(exceptions);
    }

    private static void ValidateKeyConfiguration(Type entityType, MergeEntityConfiguration mergeEntityConfiguration, List<Exception> exceptions)
    {
        var configuration = mergeEntityConfiguration.KeyConfiguration;
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

    private static void ValidateValuesConfiguration(Type entityType, MergeEntityConfiguration mergeEntityConfiguration, List<Exception> exceptions)
    {
        var configuration = mergeEntityConfiguration.ValuesConfiguration;
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
                if (mergeEntityConfiguration.KeyConfiguration?.KeyProperties != null)
                {
                    var alreadyDefinedInKey = configuration.ValuesProperties.Intersect(mergeEntityConfiguration.KeyConfiguration.KeyProperties).ToArray();
                    if (alreadyDefinedInKey.Length > 0)
                        exceptions.Add(new AlreadyDefinedPropertyException(entityType, NameOf<ValuesConfiguration>(), NameOf<KeyConfiguration>(), alreadyDefinedInKey.Select(x => x.Name)));
                }
            }
        }
    }

    private static void ValidateAdditionalValuesToCopy(Type entityType, MergeEntityConfiguration mergeEntityConfiguration, List<Exception> exceptions)
    {
        var configuration = mergeEntityConfiguration.AdditionalValuesToCopyConfiguration;
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
                if (mergeEntityConfiguration.KeyConfiguration?.KeyProperties != null)
                {
                    var alreadyDefinedInKey = configuration.AdditionalValuesToCopyProperties.Intersect(mergeEntityConfiguration.KeyConfiguration.KeyProperties).ToArray();
                    if (alreadyDefinedInKey.Length > 0)
                        exceptions.Add(new AlreadyDefinedPropertyException(entityType, NameOf<AdditionalValuesToCopyConfiguration>(), NameOf<KeyConfiguration>(), alreadyDefinedInKey.Select(x => x.Name)));
                }
                // cannot be found in values
                if (mergeEntityConfiguration.ValuesConfiguration?.ValuesProperties != null)
                {
                    var alreadyDefinedInKey = configuration.AdditionalValuesToCopyProperties.Intersect(mergeEntityConfiguration.ValuesConfiguration.ValuesProperties).ToArray();
                    if (alreadyDefinedInKey.Length > 0)
                        exceptions.Add(new AlreadyDefinedPropertyException(entityType, NameOf<AdditionalValuesToCopyConfiguration>(), NameOf<ValuesConfiguration>(), alreadyDefinedInKey.Select(x => x.Name)));
                }
            }
        }
    }

    private static void ValidateNavigationManyConfiguration(Type entityType, MergeEntityConfiguration mergeEntityConfiguration, IDictionary<Type, MergeEntityConfiguration> mergeEntityConfigurationByTypes, List<Exception> exceptions)
    {
        if (mergeEntityConfiguration.NavigationManyConfigurations != null)
        {
            foreach (var configuration in mergeEntityConfiguration.NavigationManyConfigurations)
            {
                // check if navigation child type is found in configuration
                if (!mergeEntityConfigurationByTypes.ContainsKey(configuration.NavigationManyChildType))
                    exceptions.Add(new MissingNavigationManyChildConfigurationException(entityType, configuration.NavigationManyChildType));
            }
        }
    }

    private static void ValidateNavigationOneConfiguration(Type entityType, MergeEntityConfiguration mergeEntityConfiguration, IDictionary<Type, MergeEntityConfiguration> mergeEntityConfigurationByTypes, List<Exception> exceptions)
    {
        if (mergeEntityConfiguration.NavigationOneConfigurations != null)
        {
            foreach (var configuration in mergeEntityConfiguration.NavigationOneConfigurations)
            {
                // check if navigation child type is found in configuration
                if (!mergeEntityConfigurationByTypes.ContainsKey(configuration.NavigationOneChildType))
                    exceptions.Add(new MissingNavigationOneChildConfigurationException(entityType, configuration.NavigationOneChildType));
            }
        }
    }

    private static void ValidateMarkAsConfiguration(Type entityType, MergeEntityConfiguration mergeEntityConfiguration, List<Exception> exceptions)
    {
        var configuration = mergeEntityConfiguration.MarkAsByOperation;
        ValidateMarkAsConfiguration(entityType, configuration, MergeEntityOperation.Insert, exceptions);
        ValidateMarkAsConfiguration(entityType, configuration, MergeEntityOperation.Update, exceptions);
        ValidateMarkAsConfiguration(entityType, configuration, MergeEntityOperation.Delete, exceptions);
    }

    private static void ValidateMarkAsConfiguration(Type entityType, IDictionary<MergeEntityOperation, MarkAsConfiguration> markAsConfiguration, MergeEntityOperation mergeEntityOperation, List<Exception> exceptions)
    {
        var found = markAsConfiguration.ContainsKey(mergeEntityOperation);
        if (!found)
            exceptions.Add(new MissingMarkAsConfigurationException(entityType, mergeEntityOperation));
    }

    internal static string NameOf<T>()
        => typeof(T).Name.Replace("Configuration", string.Empty);

}
