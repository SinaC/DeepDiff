using EntityMerger.Configuration;
using EntityMerger.Exceptions;
using EntityMerger.Extensions;
using System.Collections;

namespace EntityMerger;

internal sealed class Merger : IMerger
{
    private MergeConfiguration Configuration { get; }

    internal Merger(MergeConfiguration configuration)
    {
        Configuration = configuration;
    }

    // for each existing entity
    //      if entity found with same keys in new entities
    //          if values not the same
    //              copy values
    //              copy additional values
    //          merge existing.Many
    //          merge existing.One
    //          if values not the same or something modified when merging Many and One
    //              mark existing as Updated
    //      else
    //          mark existing as Deleted
    //          mark existing.Many as Deleted
    //          mark existing.One as Deleted
    //  for each new entity
    //      if entity with same keys not found in existing entities
    //          mark new as Inserted
    //          mark new.Many as Inserted
    //          mark new.One as Inserted
    public IEnumerable<TEntity> Merge<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
        where TEntity : class
    {
        if (!Configuration.MergeEntityConfigurationByTypes.TryGetValue(typeof(TEntity), out var mergeEntityConfiguration))
            throw new MissingConfigurationException(typeof(TEntity));

        var mergedEntities = Merge(mergeEntityConfiguration, existingEntities, newEntities, Configuration.UseHashtable);
        foreach (var mergedEntity in mergedEntities)
            yield return (TEntity)mergedEntity;
    }

    private bool CheckIfHashtablesShouldBeUsedUsingThreshold(IEnumerable<object> existingEntities)
        => existingEntities.Count() >= Configuration.HashtableThreshold;

    private IEnumerable<object> Merge(MergeEntityConfiguration mergeEntityConfiguration, IEnumerable<object> existingEntities, IEnumerable<object> newEntities, bool useHashtableForThatLevel)
    {
        // no entities to merge
        if ((existingEntities == null || !existingEntities.Any()) && (newEntities == null || !newEntities.Any()))
            yield break;

        // no existing entities -> return new as inserted
        if (existingEntities == null || !existingEntities.Any())
        {
            foreach (var newEntity in newEntities)
            {
                MarkEntityAndPropagateUsingNavigation(mergeEntityConfiguration, newEntity, MergeEntityOperation.Insert); // once an entity is inserted, it's children will also be inserted
                yield return newEntity;
            }
            yield break;
        }

        // no new entities -> return existing as deleted
        if (newEntities == null || !newEntities.Any())
        {
            foreach (var existingEntity in existingEntities)
            {
                MarkEntityAndPropagateUsingNavigation(mergeEntityConfiguration, existingEntity, MergeEntityOperation.Delete); // once an entity is deleted, it's children will also be deleted
                yield return existingEntity;
            }
            yield break;
        }

        // we are sure there is at least one existing and one new entity
        var existingEntitiesHashtable = useHashtableForThatLevel && Configuration.UseHashtable && CheckIfHashtablesShouldBeUsedUsingThreshold(existingEntities)
            ? InitializeHashtable(mergeEntityConfiguration.KeyConfiguration, existingEntities)
            : null;
        var newEntitiesHashtable = useHashtableForThatLevel && Configuration.UseHashtable && CheckIfHashtablesShouldBeUsedUsingThreshold(newEntities)
            ? InitializeHashtable(mergeEntityConfiguration.KeyConfiguration, newEntities)
            : null;

        // search if every existing entity is found in new entities -> this will detect update and delete
        foreach (var existingEntity in existingEntities)
        {
            var newEntity = Configuration.UseHashtable && newEntitiesHashtable != null
                ? newEntitiesHashtable[existingEntity]
                : SearchMatchingEntityByKey(mergeEntityConfiguration.KeyConfiguration, newEntities, existingEntity);
            // existing entity found in new entities -> if values are different it's an update
            if (newEntity != null)
            {
                if (mergeEntityConfiguration.ValuesConfiguration.ValuesProperties != null)
                {
                    var areNewValuesEquals = mergeEntityConfiguration.ValuesConfiguration.UsePrecompiledEqualityComparer
                        ? mergeEntityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, newEntity)
                        : mergeEntityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Equals(existingEntity, newEntity);
                    // new values are different -> copy new values and additional values to copy
                    if (!areNewValuesEquals)
                    {
                        mergeEntityConfiguration.ValuesConfiguration.ValuesProperties.CopyPropertyValues(existingEntity, newEntity);
                        mergeEntityConfiguration.AdditionalValuesToCopyConfiguration?.AdditionalValuesToCopyProperties.CopyPropertyValues(existingEntity, newEntity);
                    }

                    var mergeModificationsFound = MergeUsingNavigation(mergeEntityConfiguration, existingEntity, newEntity);
                    if (!areNewValuesEquals || mergeModificationsFound)
                    {
                        MarkEntity(mergeEntityConfiguration, existingEntity, MergeEntityOperation.Update);
                        yield return existingEntity;
                    }
                }
            }
            // existing entity not found in new entities -> it's a delete
            else
            {
                MarkEntityAndPropagateUsingNavigation(mergeEntityConfiguration, existingEntity, MergeEntityOperation.Delete); // once an entity is deleted, it's children will also be deleted
                yield return existingEntity;
            }
        }

        // search if every new entity is found in existing entities -> this will detect insert
        foreach (var newEntity in newEntities)
        {
            var newEntityFoundInExistingEntities = Configuration.UseHashtable &&  existingEntitiesHashtable != null
                ? existingEntitiesHashtable.ContainsKey(newEntity)
                : SearchMatchingEntityByKey(mergeEntityConfiguration.KeyConfiguration, existingEntities, newEntity) != null;
            // new entity not found in existing entity -> it's an insert
            if (!newEntityFoundInExistingEntities)
            {
                MarkEntityAndPropagateUsingNavigation(mergeEntityConfiguration, newEntity, MergeEntityOperation.Insert); // once an entity is inserted, it's children will also be inserted
                yield return newEntity;
            }
        }
    }

    private static object SearchMatchingEntityByKey(KeyConfiguration keyConfiguration, IEnumerable<object> entities, object existingEntity)
    {
        foreach (var entity in entities)
        {
            var areKeysEqual = keyConfiguration.UsePrecompiledEqualityComparer
                ? keyConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, entity)
                : keyConfiguration.NaiveEqualityComparer.Equals(existingEntity, entity);
            if (areKeysEqual)
                return entity;
        }
        return null!;
    }

    private static Hashtable InitializeHashtable(KeyConfiguration keyConfiguration, IEnumerable<object> entities)
    {
        var equalityComparer = keyConfiguration.UsePrecompiledEqualityComparer
            ? keyConfiguration.PrecompiledEqualityComparer
            : keyConfiguration.NaiveEqualityComparer;
        var hashtable = new Hashtable(equalityComparer);
        foreach (var entity in entities)
            hashtable.Add(entity, entity);
        return hashtable;
    }

    private bool MergeUsingNavigation(MergeEntityConfiguration mergeEntityConfiguration, object existingEntity, object newEntity)
    {
        var modificationsDetected = false;
        if (mergeEntityConfiguration.NavigationManyConfigurations != null)
        {
            foreach (var navigationManyConfiguration in mergeEntityConfiguration.NavigationManyConfigurations)
                modificationsDetected |= MergeUsingNavigationMany(navigationManyConfiguration, existingEntity, newEntity);
        }
        if (mergeEntityConfiguration.NavigationOneConfigurations != null)
        {
            foreach (var navigationOneConfiguration in mergeEntityConfiguration.NavigationOneConfigurations)
                modificationsDetected |= MergeUsingNavigationOne(navigationOneConfiguration, existingEntity, newEntity);
        }
        return modificationsDetected;
    }

    private bool MergeUsingNavigationMany(NavigationManyConfiguration navigationManyConfiguration, object existingEntity, object newEntity)
    {
        if (navigationManyConfiguration.NavigationManyProperty == null)
            return false;
        var childType = navigationManyConfiguration.NavigationManyChildType;
        if (childType == null)
            return false;

        if (!Configuration.MergeEntityConfigurationByTypes.TryGetValue(childType, out var childMergeEntityConfiguration))
            throw new MissingConfigurationException(childType);

        var existingEntityChildren = navigationManyConfiguration.NavigationManyProperty.GetValue(existingEntity);
        var newEntityChildren = navigationManyConfiguration.NavigationManyProperty.GetValue(newEntity);

        // merge children
        var existingChildren = (IEnumerable<object>)existingEntityChildren!;
        var newChildren = (IEnumerable<object>)newEntityChildren!;
        var mergedChildren = Merge(childMergeEntityConfiguration, existingChildren, newChildren, navigationManyConfiguration.UseHashtable);

        // convert children from IEnumerable<object> to List<ChildType>
        var listType = typeof(List<>).MakeGenericType(childType);
        var list = (IList)Activator.CreateInstance(listType)!;
        foreach (var mergedChild in mergedChildren)
            list.Add(mergedChild);
        if (list.Count > 0)
        {
            navigationManyConfiguration.NavigationManyProperty.SetValue(existingEntity, list);
            return true;
        }
        return false;
    }

    private bool MergeUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object existingEntity, object newEntity)
    {
        if (navigationOneConfiguration.NavigationOneProperty == null)
            return false;
        var childType = navigationOneConfiguration.NavigationOneChildType;
        if (childType == null)
            return false;

        if (!Configuration.MergeEntityConfigurationByTypes.TryGetValue(childType, out var childMergeEntityConfiguration))
            throw new MissingConfigurationException(childType);

        var existingEntityChild = navigationOneConfiguration.NavigationOneProperty.GetValue(existingEntity);
        var newEntityChild = navigationOneConfiguration.NavigationOneProperty.GetValue(newEntity);

        // was not existing and is now new -> it's an insert
        if (existingEntityChild == null && newEntityChild != null)
        {
            navigationOneConfiguration.NavigationOneProperty.SetValue(existingEntity, newEntityChild);
            MarkEntityAndPropagateUsingNavigation(childMergeEntityConfiguration, newEntityChild, MergeEntityOperation.Insert);
            return true;
        }
        // was existing and is not new -> it's a delete
        if (existingEntityChild != null && newEntityChild == null)
        {
            MarkEntityAndPropagateUsingNavigation(childMergeEntityConfiguration, existingEntityChild, MergeEntityOperation.Delete);
            return true;
        }
        // was existing and is new -> maybe an update
        if (existingEntityChild != null && newEntityChild != null)
        {
            bool areKeysEqual = false;
            if (childMergeEntityConfiguration.KeyConfiguration.KeyProperties != null)
            {
                areKeysEqual = childMergeEntityConfiguration.KeyConfiguration.UsePrecompiledEqualityComparer
                    ? childMergeEntityConfiguration.KeyConfiguration.PrecompiledEqualityComparer.Equals(existingEntityChild, newEntityChild)
                    : childMergeEntityConfiguration.KeyConfiguration.NaiveEqualityComparer.Equals(existingEntityChild, newEntityChild);
                if (!areKeysEqual) // keys are different -> copy keys
                    childMergeEntityConfiguration.KeyConfiguration.KeyProperties.CopyPropertyValues(existingEntityChild, newEntityChild);
            }

            bool areNewValuesEquals = false;
            if (childMergeEntityConfiguration.ValuesConfiguration.ValuesProperties != null)
            {
                areNewValuesEquals = childMergeEntityConfiguration.ValuesConfiguration.UsePrecompiledEqualityComparer
                    ? childMergeEntityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Equals(existingEntityChild, newEntityChild)
                    : childMergeEntityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Equals(existingEntityChild, newEntityChild);
                // new values are different -> copy new values and additional values to copy
                if (!areNewValuesEquals)
                {
                    childMergeEntityConfiguration.ValuesConfiguration.ValuesProperties.CopyPropertyValues(existingEntityChild, newEntityChild);
                    childMergeEntityConfiguration.AdditionalValuesToCopyConfiguration?.AdditionalValuesToCopyProperties.CopyPropertyValues(existingEntityChild, newEntityChild);
                }
            }

            var mergeModificationsFound = MergeUsingNavigation(childMergeEntityConfiguration, existingEntityChild, newEntityChild);

            if (!areKeysEqual || !areNewValuesEquals || mergeModificationsFound)
            {
                MarkEntity(childMergeEntityConfiguration, existingEntityChild, MergeEntityOperation.Update);
                return true;
            }
        }
        return false;
    }

    private static void MarkEntity(MergeEntityConfiguration mergeEntityConfiguration, object entity, MergeEntityOperation operation)
    {
        if (!mergeEntityConfiguration.MarkAsByOperation.TryGetValue(operation, out var markAsConfiguration))
            throw new MissingMarkAsConfigurationException(entity.GetType(), operation);
        markAsConfiguration.DestinationProperty.SetValue(entity, markAsConfiguration.Value);
    }

    private void MarkEntityAndPropagateUsingNavigation(MergeEntityConfiguration mergeEntityConfiguration, object entity, MergeEntityOperation operation)
    {
        MarkEntity(mergeEntityConfiguration, entity, operation);
        PropagateUsingNavigation(mergeEntityConfiguration, entity, operation);
    }

    private void PropagateUsingNavigation(MergeEntityConfiguration mergeEntityConfiguration, object entity, MergeEntityOperation operation)
    {
        if (mergeEntityConfiguration.NavigationManyConfigurations != null)
        {
            foreach (var navigationManyConfiguration in mergeEntityConfiguration.NavigationManyConfigurations)
                PropagateUsingNavigationMany(navigationManyConfiguration, entity, operation);
        }
        if (mergeEntityConfiguration.NavigationOneConfigurations != null)
        {
            foreach (var navigationOneConfiguration in mergeEntityConfiguration.NavigationOneConfigurations)
                PropagateUsingNavigationOne(navigationOneConfiguration, entity, operation);
        }
    }

    private void PropagateUsingNavigationMany(NavigationManyConfiguration navigationManyConfiguration, object entity, MergeEntityOperation operation)
    {
        if (navigationManyConfiguration.NavigationManyProperty == null)
            return;
        var childrenValue = navigationManyConfiguration.NavigationManyProperty.GetValue(entity);
        if (childrenValue == null)
            return;
        var childType = navigationManyConfiguration.NavigationManyChildType;
        if (childType == null)
            return;
        if (!Configuration.MergeEntityConfigurationByTypes.TryGetValue(childType, out var childMergeEntityConfiguration))
            throw new MissingConfigurationException(childType);
        if (!childMergeEntityConfiguration.MarkAsByOperation.TryGetValue(operation, out var markAsConfiguration))
            throw new MissingMarkAsConfigurationException(entity.GetType(), operation);
        var children = (IEnumerable<object>)childrenValue;
        foreach (var child in children)
            MarkEntityAndPropagateUsingNavigation(childMergeEntityConfiguration, child, operation);
    }

    private void PropagateUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object entity, MergeEntityOperation operation)
    {
        if (navigationOneConfiguration.NavigationOneProperty == null)
            return;
        var childValue = navigationOneConfiguration.NavigationOneProperty.GetValue(entity);
        if (childValue == null)
            return;
        var childType = navigationOneConfiguration.NavigationOneChildType;
        if (childType == null)
            return;
        if (!Configuration.MergeEntityConfigurationByTypes.TryGetValue(childType, out var childMergeEntityConfiguration))
            throw new MissingConfigurationException(childType);
        MarkEntityAndPropagateUsingNavigation(childMergeEntityConfiguration, childValue, operation);
    }
}
