using EntityComparer.Configuration;
using EntityComparer.Exceptions;
using EntityComparer.Extensions;
using System.Collections;

namespace EntityComparer;

internal sealed class EntityComparer : IEntityComparer
{
    private CompareConfiguration Configuration { get; }

    internal EntityComparer(CompareConfiguration configuration)
    {
        Configuration = configuration;
    }

    // for each existing entity
    //      if entity found with same keys in new entities
    //          if values not the same
    //              copy values
    //              copy additional values
    //          compare existing.Many
    //          compare existing.One
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
    public IEnumerable<TEntity> Compare<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
        where TEntity : class
    {
        if (!Configuration.CompareEntityConfigurationByTypes.TryGetValue(typeof(TEntity), out var compareEntityConfiguration))
            throw new MissingConfigurationException(typeof(TEntity));

        var comparedEntities = Compare(compareEntityConfiguration, existingEntities, newEntities, Configuration.UseHashtable);
        foreach (var comparedEntity in comparedEntities)
            yield return (TEntity)comparedEntity;
    }

    private bool CheckIfHashtablesShouldBeUsedUsingThreshold(IEnumerable<object> existingEntities)
        => existingEntities.Count() >= Configuration.HashtableThreshold;

    private IEnumerable<object> Compare(CompareEntityConfiguration compareEntityConfiguration, IEnumerable<object> existingEntities, IEnumerable<object> newEntities, bool useHashtableForThatLevel)
    {
        // no entities to compare
        if ((existingEntities == null || !existingEntities.Any()) && (newEntities == null || !newEntities.Any()))
            yield break;

        // no existing entities -> return new as inserted
        if (existingEntities == null || !existingEntities.Any())
        {
            foreach (var newEntity in newEntities)
            {
                MarkEntityAndPropagateUsingNavigation(compareEntityConfiguration, newEntity, CompareEntityOperation.Insert); // once an entity is inserted, it's children will also be inserted
                yield return newEntity;
            }
            yield break;
        }

        // no new entities -> return existing as deleted
        if (newEntities == null || !newEntities.Any())
        {
            foreach (var existingEntity in existingEntities)
            {
                MarkEntityAndPropagateUsingNavigation(compareEntityConfiguration, existingEntity, CompareEntityOperation.Delete); // once an entity is deleted, it's children will also be deleted
                yield return existingEntity;
            }
            yield break;
        }

        // we are sure there is at least one existing and one new entity
        var existingEntitiesHashtable = useHashtableForThatLevel && Configuration.UseHashtable && CheckIfHashtablesShouldBeUsedUsingThreshold(existingEntities)
            ? InitializeHashtable(compareEntityConfiguration.KeyConfiguration, existingEntities)
            : null;
        var newEntitiesHashtable = useHashtableForThatLevel && Configuration.UseHashtable && CheckIfHashtablesShouldBeUsedUsingThreshold(newEntities)
            ? InitializeHashtable(compareEntityConfiguration.KeyConfiguration, newEntities)
            : null;

        // search if every existing entity is found in new entities -> this will detect update and delete
        foreach (var existingEntity in existingEntities)
        {
            var newEntity = Configuration.UseHashtable && newEntitiesHashtable != null
                ? newEntitiesHashtable[existingEntity]
                : SearchMatchingEntityByKey(compareEntityConfiguration.KeyConfiguration, newEntities, existingEntity);
            // existing entity found in new entities -> if values are different it's an update
            if (newEntity != null)
            {
                if (compareEntityConfiguration.ValuesConfiguration.ValuesProperties != null)
                {
                    var areNewValuesEquals = compareEntityConfiguration.ValuesConfiguration.UsePrecompiledEqualityComparer
                        ? compareEntityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, newEntity)
                        : compareEntityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Equals(existingEntity, newEntity);
                    // new values are different -> copy new values and additional values to copy
                    if (!areNewValuesEquals)
                    {
                        compareEntityConfiguration.ValuesConfiguration.ValuesProperties.CopyPropertyValues(existingEntity, newEntity);
                        compareEntityConfiguration.AdditionalValuesToCopyConfiguration?.AdditionalValuesToCopyProperties.CopyPropertyValues(existingEntity, newEntity);
                    }

                    var compareModificationsFound = CompareUsingNavigation(compareEntityConfiguration, existingEntity, newEntity);
                    if (!areNewValuesEquals || compareModificationsFound)
                    {
                        MarkEntity(compareEntityConfiguration, existingEntity, CompareEntityOperation.Update);
                        yield return existingEntity;
                    }
                }
            }
            // existing entity not found in new entities -> it's a delete
            else
            {
                MarkEntityAndPropagateUsingNavigation(compareEntityConfiguration, existingEntity, CompareEntityOperation.Delete); // once an entity is deleted, it's children will also be deleted
                yield return existingEntity;
            }
        }

        // search if every new entity is found in existing entities -> this will detect insert
        foreach (var newEntity in newEntities)
        {
            var newEntityFoundInExistingEntities = Configuration.UseHashtable &&  existingEntitiesHashtable != null
                ? existingEntitiesHashtable.ContainsKey(newEntity)
                : SearchMatchingEntityByKey(compareEntityConfiguration.KeyConfiguration, existingEntities, newEntity) != null;
            // new entity not found in existing entity -> it's an insert
            if (!newEntityFoundInExistingEntities)
            {
                MarkEntityAndPropagateUsingNavigation(compareEntityConfiguration, newEntity, CompareEntityOperation.Insert); // once an entity is inserted, it's children will also be inserted
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

    private bool CompareUsingNavigation(CompareEntityConfiguration compareEntityConfiguration, object existingEntity, object newEntity)
    {
        var modificationsDetected = false;
        if (compareEntityConfiguration.NavigationManyConfigurations != null)
        {
            foreach (var navigationManyConfiguration in compareEntityConfiguration.NavigationManyConfigurations)
                modificationsDetected |= CompareUsingNavigationMany(navigationManyConfiguration, existingEntity, newEntity);
        }
        if (compareEntityConfiguration.NavigationOneConfigurations != null)
        {
            foreach (var navigationOneConfiguration in compareEntityConfiguration.NavigationOneConfigurations)
                modificationsDetected |= CompareUsingNavigationOne(navigationOneConfiguration, existingEntity, newEntity);
        }
        return modificationsDetected;
    }

    private bool CompareUsingNavigationMany(NavigationManyConfiguration navigationManyConfiguration, object existingEntity, object newEntity)
    {
        if (navigationManyConfiguration.NavigationManyProperty == null)
            return false;
        var childType = navigationManyConfiguration.NavigationManyChildType;
        if (childType == null)
            return false;

        if (!Configuration.CompareEntityConfigurationByTypes.TryGetValue(childType, out var childCompareEntityConfiguration))
            throw new MissingConfigurationException(childType);

        var existingEntityChildren = navigationManyConfiguration.NavigationManyProperty.GetValue(existingEntity);
        var newEntityChildren = navigationManyConfiguration.NavigationManyProperty.GetValue(newEntity);

        // compare children
        var existingChildren = (IEnumerable<object>)existingEntityChildren!;
        var newChildren = (IEnumerable<object>)newEntityChildren!;
        var comparedChildren = Compare(childCompareEntityConfiguration, existingChildren, newChildren, navigationManyConfiguration.UseHashtable);

        // convert compared children from IEnumerable<object> to List<ChildType>
        var listType = typeof(List<>).MakeGenericType(childType);
        var list = (IList)Activator.CreateInstance(listType)!;
        foreach (var comparedChild in comparedChildren)
            list.Add(comparedChild);
        navigationManyConfiguration.NavigationManyProperty.SetValue(existingEntity, list);
        if (list.Count > 0)
            return true;
        return false;
    }

    private bool CompareUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object existingEntity, object newEntity)
    {
        if (navigationOneConfiguration.NavigationOneProperty == null)
            return false;
        var childType = navigationOneConfiguration.NavigationOneChildType;
        if (childType == null)
            return false;

        if (!Configuration.CompareEntityConfigurationByTypes.TryGetValue(childType, out var childCompareEntityConfiguration))
            throw new MissingConfigurationException(childType);

        var existingEntityChild = navigationOneConfiguration.NavigationOneProperty.GetValue(existingEntity);
        var newEntityChild = navigationOneConfiguration.NavigationOneProperty.GetValue(newEntity);

        // was not existing and is now new -> it's an insert
        if (existingEntityChild == null && newEntityChild != null)
        {
            navigationOneConfiguration.NavigationOneProperty.SetValue(existingEntity, newEntityChild);
            MarkEntityAndPropagateUsingNavigation(childCompareEntityConfiguration, newEntityChild, CompareEntityOperation.Insert);
            return true;
        }
        // was existing and is not new -> it's a delete
        if (existingEntityChild != null && newEntityChild == null)
        {
            MarkEntityAndPropagateUsingNavigation(childCompareEntityConfiguration, existingEntityChild, CompareEntityOperation.Delete);
            return true;
        }
        // was existing and is new -> maybe an update
        if (existingEntityChild != null && newEntityChild != null)
        {
            bool areKeysEqual = false;
            if (childCompareEntityConfiguration.KeyConfiguration.KeyProperties != null)
            {
                areKeysEqual = childCompareEntityConfiguration.KeyConfiguration.UsePrecompiledEqualityComparer
                    ? childCompareEntityConfiguration.KeyConfiguration.PrecompiledEqualityComparer.Equals(existingEntityChild, newEntityChild)
                    : childCompareEntityConfiguration.KeyConfiguration.NaiveEqualityComparer.Equals(existingEntityChild, newEntityChild);
                if (!areKeysEqual) // keys are different -> copy keys
                    childCompareEntityConfiguration.KeyConfiguration.KeyProperties.CopyPropertyValues(existingEntityChild, newEntityChild);
            }

            bool areNewValuesEquals = false;
            if (childCompareEntityConfiguration.ValuesConfiguration.ValuesProperties != null)
            {
                areNewValuesEquals = childCompareEntityConfiguration.ValuesConfiguration.UsePrecompiledEqualityComparer
                    ? childCompareEntityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Equals(existingEntityChild, newEntityChild)
                    : childCompareEntityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Equals(existingEntityChild, newEntityChild);
                // new values are different -> copy new values and additional values to copy
                if (!areNewValuesEquals)
                {
                    childCompareEntityConfiguration.ValuesConfiguration.ValuesProperties.CopyPropertyValues(existingEntityChild, newEntityChild);
                    childCompareEntityConfiguration.AdditionalValuesToCopyConfiguration?.AdditionalValuesToCopyProperties.CopyPropertyValues(existingEntityChild, newEntityChild);
                }
            }

            var compareModificationsFound = CompareUsingNavigation(childCompareEntityConfiguration, existingEntityChild, newEntityChild);

            if (!areKeysEqual || !areNewValuesEquals || compareModificationsFound)
            {
                MarkEntity(childCompareEntityConfiguration, existingEntityChild, CompareEntityOperation.Update);
                return true;
            }
        }
        return false;
    }

    private static void MarkEntity(CompareEntityConfiguration compareEntityConfiguration, object entity, CompareEntityOperation operation)
    {
        if (!compareEntityConfiguration.MarkAsByOperation.TryGetValue(operation, out var markAsConfiguration))
            throw new MissingMarkAsConfigurationException(entity.GetType(), operation);
        markAsConfiguration.DestinationProperty.SetValue(entity, markAsConfiguration.Value);
    }

    private void MarkEntityAndPropagateUsingNavigation(CompareEntityConfiguration compareEntityConfiguration, object entity, CompareEntityOperation operation)
    {
        MarkEntity(compareEntityConfiguration, entity, operation);
        PropagateUsingNavigation(compareEntityConfiguration, entity, operation);
    }

    private void PropagateUsingNavigation(CompareEntityConfiguration compareEntityConfiguration, object entity, CompareEntityOperation operation)
    {
        if (compareEntityConfiguration.NavigationManyConfigurations != null)
        {
            foreach (var navigationManyConfiguration in compareEntityConfiguration.NavigationManyConfigurations)
                PropagateUsingNavigationMany(navigationManyConfiguration, entity, operation);
        }
        if (compareEntityConfiguration.NavigationOneConfigurations != null)
        {
            foreach (var navigationOneConfiguration in compareEntityConfiguration.NavigationOneConfigurations)
                PropagateUsingNavigationOne(navigationOneConfiguration, entity, operation);
        }
    }

    private void PropagateUsingNavigationMany(NavigationManyConfiguration navigationManyConfiguration, object entity, CompareEntityOperation operation)
    {
        if (navigationManyConfiguration.NavigationManyProperty == null)
            return;
        var childrenValue = navigationManyConfiguration.NavigationManyProperty.GetValue(entity);
        if (childrenValue == null)
            return;
        var childType = navigationManyConfiguration.NavigationManyChildType;
        if (childType == null)
            return;
        if (!Configuration.CompareEntityConfigurationByTypes.TryGetValue(childType, out var childCompareEntityConfiguration))
            throw new MissingConfigurationException(childType);
        if (!childCompareEntityConfiguration.MarkAsByOperation.TryGetValue(operation, out var markAsConfiguration))
            throw new MissingMarkAsConfigurationException(entity.GetType(), operation);
        var children = (IEnumerable<object>)childrenValue;
        foreach (var child in children)
            MarkEntityAndPropagateUsingNavigation(childCompareEntityConfiguration, child, operation);
    }

    private void PropagateUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object entity, CompareEntityOperation operation)
    {
        if (navigationOneConfiguration.NavigationOneProperty == null)
            return;
        var childValue = navigationOneConfiguration.NavigationOneProperty.GetValue(entity);
        if (childValue == null)
            return;
        var childType = navigationOneConfiguration.NavigationOneChildType;
        if (childType == null)
            return;
        if (!Configuration.CompareEntityConfigurationByTypes.TryGetValue(childType, out var childCompareEntityConfiguration))
            throw new MissingConfigurationException(childType);
        MarkEntityAndPropagateUsingNavigation(childCompareEntityConfiguration, childValue, operation);
    }
}
