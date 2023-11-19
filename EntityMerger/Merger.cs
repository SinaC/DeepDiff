using EntityMerger.Configuration;
using EntityMerger.Extensions;
using System.Collections;

namespace EntityMerger;

internal class Merger : IMerger
{
    private MergeConfiguration Configuration { get; }

    internal Merger(MergeConfiguration configuration)
    {
        Configuration = configuration;
    }

    // for each existing
    //      if entity found with same keys in existing
    //          if values not the same
    //              copy values TODO: additional values to copy which are not included in valueProperties
    //          merge existing.Many
    //          merge existing.One
    //          if values not the same or something modified when merging Many and One
    //              mark existing as Updated
    //      else
    //          mark existing as Deleted
    //          mark existing.Many as Deleted
    //          mark existing.One as Deleted
    //  for each calculated
    //      if entity with same keys not found in existing
    //          mark calculated as Inserted
    //          mark calculated.Many as Inserted
    //          mark calculated.One as Inserted
    public IEnumerable<TEntity> Merge<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> calculatedEntities)
        where TEntity : class
    {
        if (!Configuration.MergeEntityConfigurations.TryGetValue(typeof(TEntity), out var mergeEntityConfiguration))
            yield break; // TODO: exception

        var mergedEntities = Merge(mergeEntityConfiguration, existingEntities, calculatedEntities, Configuration.UseHashtable);
        foreach (var mergedEntity in mergedEntities)
            yield return (TEntity)mergedEntity;
    }

    private IEnumerable<object> Merge(MergeEntityConfiguration mergeEntityConfiguration, IEnumerable<object> existingEntities, IEnumerable<object> calculatedEntities, bool useHashtable)
    {
        var existingEntitiesHashtable = useHashtable
            ? InitializeHashtable(mergeEntityConfiguration.KeyConfiguration, existingEntities)
            : null;
        var calculatedEntitiesHashtable = useHashtable
            ? InitializeHashtable(mergeEntityConfiguration.KeyConfiguration, calculatedEntities)
            : null;

        // search if every existing entity is found in calculated entities -> this will detect update and delete
        foreach (var existingEntity in existingEntities)
        {
            var calculatedEntity = Configuration.UseHashtable && calculatedEntitiesHashtable != null
                ? calculatedEntitiesHashtable[existingEntity]
                : SearchMatchingEntityByKey(mergeEntityConfiguration.KeyConfiguration, calculatedEntities, existingEntity);
            // existing entity found in calculated entities -> if values are different it's an update
            if (calculatedEntity != null)
            {
                if (mergeEntityConfiguration.CalculatedValueConfiguration.CalculatedValueProperties != null)
                {
                    var areCalculatedValuesEquals = mergeEntityConfiguration.CalculatedValueConfiguration.UsePrecompiledEqualityComparer
                        ? mergeEntityConfiguration.CalculatedValueConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, calculatedEntity)
                        : mergeEntityConfiguration.CalculatedValueConfiguration.NaiveEqualityComparer.Equals(existingEntity, calculatedEntity);
                    if (!areCalculatedValuesEquals) // calculated values are different -> copy calculated values
                        mergeEntityConfiguration.CalculatedValueConfiguration.CalculatedValueProperties.CopyPropertyValues(existingEntity, calculatedEntity);

                    var mergeModificationsFound = MergeUsingNavigation(mergeEntityConfiguration, existingEntity, calculatedEntity);
                    if (!areCalculatedValuesEquals || mergeModificationsFound)
                    {
                        MarkEntity(mergeEntityConfiguration, existingEntity, MergeEntityOperation.Update);
                        yield return existingEntity;
                    }
                }
            }
            // existing entity not found in calculated entities -> it's a delete
            else
            {
                MarkEntityAndPropagateUsingNavigation(mergeEntityConfiguration, existingEntity, MergeEntityOperation.Delete); // once an entity is deleted, it's children will also be deleted
                yield return existingEntity;
            }
        }

        // search if every calculated entity is found in existing entities -> this will detect insert
        foreach (var calculatedEntity in calculatedEntities)
        {
            var calculatedEntityFoundInExistingEntities = Configuration.UseHashtable &&  existingEntitiesHashtable != null
                ? existingEntitiesHashtable.ContainsKey(calculatedEntity)
                : SearchMatchingEntityByKey(mergeEntityConfiguration.KeyConfiguration, existingEntities, calculatedEntity) != null;
            // calculated entity not found in existing entity -> it's an insert
            if (!calculatedEntityFoundInExistingEntities)
            {
                MarkEntityAndPropagateUsingNavigation(mergeEntityConfiguration, calculatedEntity, MergeEntityOperation.Insert); // once an entity is inserted, it's children will also be inserted
                yield return calculatedEntity;
            }
        }
    }

    private object SearchMatchingEntityByKey(KeyConfiguration keyConfiguration, IEnumerable<object> entities, object existingEntity)
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

    private Hashtable InitializeHashtable(KeyConfiguration keyConfiguration, IEnumerable<object> entities)
    {
        var equalityComparer = keyConfiguration.UsePrecompiledEqualityComparer
            ? keyConfiguration.PrecompiledEqualityComparer
            : keyConfiguration.NaiveEqualityComparer;
        var hashtable = new Hashtable(equalityComparer);
        foreach (var entity in entities)
            hashtable.Add(entity, entity);
        return hashtable;
    }

    private bool MergeUsingNavigation(MergeEntityConfiguration mergeEntityConfiguration, object existingEntity, object calculatedEntity)
    {
        var modificationsDetected = false;
        if (mergeEntityConfiguration.NavigationManyConfigurations != null)
        {
            foreach (var navigationManyConfiguration in mergeEntityConfiguration.NavigationManyConfigurations)
                modificationsDetected |= MergeUsingNavigationMany(navigationManyConfiguration, existingEntity, calculatedEntity);
        }
        if (mergeEntityConfiguration.NavigationOneConfigurations != null)
        {
            foreach (var navigationOneConfiguration in mergeEntityConfiguration.NavigationOneConfigurations)
                modificationsDetected |= MergeUsingNavigationOne(navigationOneConfiguration, existingEntity, calculatedEntity);
        }
        return modificationsDetected;
    }

    private bool MergeUsingNavigationMany(NavigationManyConfiguration navigationManyConfiguration, object existingEntity, object calculatedEntity)
    {
        if (navigationManyConfiguration.NavigationManyProperty == null)
            return false;
        var childType = navigationManyConfiguration.NavigationManyChildType;
        if (childType == null)
            return false;

        if (!Configuration.MergeEntityConfigurations.TryGetValue(childType, out var childMergeEntityConfiguration))
            return false; // TODO: exception

        var existingEntityChildren = navigationManyConfiguration.NavigationManyProperty.GetValue(existingEntity);
        var calculatedEntityChildren = navigationManyConfiguration.NavigationManyProperty.GetValue(calculatedEntity);

        // merge children
        var existingChildren = (IEnumerable<object>)existingEntityChildren!;
        var calculatedChildren = (IEnumerable<object>)calculatedEntityChildren!;
        var mergedChildren = Merge(childMergeEntityConfiguration, existingChildren, calculatedChildren, Configuration.UseHashtable && navigationManyConfiguration.UseHashtable);

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

    private bool MergeUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object existingEntity, object calculatedEntity)
    {
        if (navigationOneConfiguration.NavigationOneProperty == null)
            return false;
        var childType = navigationOneConfiguration.NavigationOneProperty.PropertyType;
        if (childType == null)
            return false;

        if (!Configuration.MergeEntityConfigurations.TryGetValue(childType, out var childMergeEntityConfiguration))
            return false; // TODO: exception

        var existingEntityChild = navigationOneConfiguration.NavigationOneProperty.GetValue(existingEntity);
        var calculatedEntityChild = navigationOneConfiguration.NavigationOneProperty.GetValue(calculatedEntity);

        // was not existing and is now calculated -> it's an insert
        if (existingEntityChild == null && calculatedEntityChild != null)
        {
            navigationOneConfiguration.NavigationOneProperty.SetValue(existingEntity, calculatedEntityChild);
            MarkEntityAndPropagateUsingNavigation(childMergeEntityConfiguration, calculatedEntityChild, MergeEntityOperation.Insert);
            return true;
        }
        // was existing and is not calculated -> it's a delete
        if (existingEntityChild != null && calculatedEntityChild == null)
        {
            MarkEntityAndPropagateUsingNavigation(childMergeEntityConfiguration, existingEntityChild, MergeEntityOperation.Delete);
            return true;
        }
        // was existing and is calculated -> maybe an update
        if (existingEntityChild != null && calculatedEntityChild != null)
        {
            bool areKeysEqual = false;
            if (childMergeEntityConfiguration.KeyConfiguration.KeyProperties != null)
            {
                areKeysEqual = childMergeEntityConfiguration.KeyConfiguration.UsePrecompiledEqualityComparer
                    ? childMergeEntityConfiguration.KeyConfiguration.PrecompiledEqualityComparer.Equals(existingEntityChild, calculatedEntityChild)
                    : childMergeEntityConfiguration.KeyConfiguration.NaiveEqualityComparer.Equals(existingEntityChild, calculatedEntityChild);
                if (!areKeysEqual) // keys are different -> copy keys
                    childMergeEntityConfiguration.KeyConfiguration.KeyProperties.CopyPropertyValues(existingEntityChild, calculatedEntityChild);
            }

            bool areCalculatedValuesEquals = false;
            if (childMergeEntityConfiguration.CalculatedValueConfiguration.CalculatedValueProperties != null)
            {
                areCalculatedValuesEquals = childMergeEntityConfiguration.CalculatedValueConfiguration.UsePrecompiledEqualityComparer
                    ? childMergeEntityConfiguration.CalculatedValueConfiguration.PrecompiledEqualityComparer.Equals(existingEntityChild, calculatedEntityChild)
                    : childMergeEntityConfiguration.CalculatedValueConfiguration.NaiveEqualityComparer.Equals(existingEntityChild, calculatedEntityChild);
                if (!areCalculatedValuesEquals) // calculated values are different -> copy calculated values
                    childMergeEntityConfiguration.CalculatedValueConfiguration.CalculatedValueProperties.CopyPropertyValues(existingEntityChild, calculatedEntityChild);
            }

            var mergeModificationsFound = MergeUsingNavigation(childMergeEntityConfiguration, existingEntityChild, calculatedEntityChild);

            if (!areKeysEqual || !areCalculatedValuesEquals || mergeModificationsFound)
            {
                MarkEntity(childMergeEntityConfiguration, existingEntityChild, MergeEntityOperation.Update);
                return true;
            }
        }
        return false;
    }

    private void MarkEntity(MergeEntityConfiguration mergeEntityConfiguration, object entity, MergeEntityOperation operation)
    {
        var assignValue = mergeEntityConfiguration.MarkAsByOperation[operation];
        if (assignValue != null)
            assignValue.DestinationProperty.SetValue(entity, assignValue.Value);
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
        var assignValue = Configuration.MergeEntityConfigurations[childType].MarkAsByOperation[operation];
        if (assignValue != null)
        {
            var children = (IEnumerable<object>)childrenValue;
            foreach (var child in children)
                assignValue.DestinationProperty.SetValue(child, assignValue.Value);
        }
    }

    private void PropagateUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object entity, MergeEntityOperation operation)
    {
        if (navigationOneConfiguration.NavigationOneProperty == null)
            return;
        var childValue = navigationOneConfiguration.NavigationOneProperty.GetValue(entity);
        if (childValue == null)
            return;
        var childType = navigationOneConfiguration.NavigationOneProperty.PropertyType;
        if (childType == null)
            return;
        var assignValue = Configuration.MergeEntityConfigurations[childType].MarkAsByOperation[operation];
        if (assignValue != null)
            assignValue.DestinationProperty.SetValue(childValue, assignValue.Value);
    }
}
