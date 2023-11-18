using EntityMerger.EntityMerger;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace EntityMerger;

public class Merger : IMerger
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

        var mergedEntities = Merge(mergeEntityConfiguration, existingEntities, calculatedEntities);
        foreach (var mergedEntity in mergedEntities)
            yield return (TEntity)mergedEntity;
    }

    public bool Equals<TEntity>(TEntity entity1, TEntity entity2)
    {
        var mergeEntityConfiguration = Configuration.MergeEntityConfigurations[typeof(TEntity)];
        return mergeEntityConfiguration.KeyConfiguration.UsePrecompiledEqualityComparer
            ? mergeEntityConfiguration.KeyConfiguration.PrecompiledEqualityComparer.Equals(entity1, entity2)
            : mergeEntityConfiguration.KeyConfiguration.NaiveEqualityComparer.Equals(entity1, entity2);
    }

    //private IEnumerable<object> MergeWithHashtable(MergeEntityConfiguration mergeEntityConfiguration, IEnumerable<object> existingEntities, IEnumerable<object> calculatedEntities)
    //{
    //    var existingEntitiesHashtable = InitializeHashtable(mergeEntityConfiguration, existingEntities);
    //    var calculatedEntitiesHashtable = InitializeHashtable(mergeEntityConfiguration, calculatedEntities);
    //    // search if every existing entity is found in calculated entities -> this will detect update and delete
    //    foreach (var existingEntity in existingEntities)
    //    {
    //        var calculatedEntity = calculatedEntitiesHashtable[existingEntity];

    //        // existing entity found in calculated entities -> if values are different it's an update
    //        if (calculatedEntity != null)
    //        {
    //            if (mergeEntityConfiguration.CalculatedValueConfiguration.CalculatedValueProperties != null)
    //            {
    //                var areCalculatedValuesEquals = mergeEntityConfiguration.CalculatedValueConfiguration.UsePrecompileEqualityComparer
    //                    ? mergeEntityConfiguration.CalculatedValueConfiguration.EqualityComparer.Equals(existingEntity, calculatedEntity)
    //                    : mergeEntityConfiguration.CalculatedValueConfiguration.CalculatedValueProperties.Equals(existingEntity, calculatedEntity);
    //                if (!areCalculatedValuesEquals) // calculated values are different -> copy calculated values
    //                    mergeEntityConfiguration.CalculatedValueConfiguration.CalculatedValueProperties.CopyPropertyValues(existingEntity, calculatedEntity);

    //                var mergeModificationsFound = MergeUsingNavigation(mergeEntityConfiguration, existingEntity, calculatedEntity);
    //                if (!areCalculatedValuesEquals || mergeModificationsFound)
    //                {
    //                    MarkEntity(mergeEntityConfiguration, existingEntity, MergeEntityOperation.Update);
    //                    yield return existingEntity;
    //                }
    //            }
    //        }
    //        // existing entity not found in calculated entities -> it's a delete
    //        else
    //        {
    //            MarkEntityAndPropagateUsingNavigation(mergeEntityConfiguration, existingEntity, MergeEntityOperation.Delete); // once an entity is deleted, it's children will also be deleted
    //            yield return existingEntity;
    //        }
    //    }

    //    // search if every calculated entity is found in existing entities -> this will detect insert
    //    foreach (var calculatedEntity in calculatedEntities)
    //    {
    //        var calculatedEntityFoundInExistingEntities = existingEntitiesHashtable.ContainsKey(calculatedEntity);
    //        // calculated entity not found in existing entity -> it's an insert
    //        if (!calculatedEntityFoundInExistingEntities)
    //        {
    //            MarkEntityAndPropagateUsingNavigation(mergeEntityConfiguration, calculatedEntity, MergeEntityOperation.Insert); // once an entity is inserted, it's children will also be inserted
    //            yield return calculatedEntity;
    //        }
    //    }
    //}

    private IEnumerable<object> Merge(MergeEntityConfiguration mergeEntityConfiguration, IEnumerable<object> existingEntities, IEnumerable<object> calculatedEntities)
    {
        // search if every existing entity is found in calculated entities -> this will detect update and delete
        foreach (var existingEntity in existingEntities)
        {
            var existingEntityFoundInCalculatedEntities = false;
            foreach (var calculatedEntity in calculatedEntities)
            {
                if (mergeEntityConfiguration.KeyConfiguration.KeyProperties != null)
                {
                    var areKeysEqual = mergeEntityConfiguration.KeyConfiguration.UsePrecompiledEqualityComparer
                        ? mergeEntityConfiguration.KeyConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, calculatedEntity)
                        : mergeEntityConfiguration.KeyConfiguration.NaiveEqualityComparer.Equals(existingEntity, calculatedEntity);
                    // existing entity found in calculated entities -> if values are different it's an update
                    if (areKeysEqual)
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

                            existingEntityFoundInCalculatedEntities = true;
                            break; // don't need to check other calculated entities
                        }
                    }
                }
            }
            // existing entity not found in calculated entities -> it's a delete
            if (!existingEntityFoundInCalculatedEntities)
            {
                MarkEntityAndPropagateUsingNavigation(mergeEntityConfiguration, existingEntity, MergeEntityOperation.Delete); // once an entity is deleted, it's children will also be deleted
                yield return existingEntity;
            }
        }

        // search if every calculated entity is found in existing entities -> this will detect insert
        foreach (var calculatedEntity in calculatedEntities)
        {
            var calculatedEntityFoundInExistingEntities = false;
            foreach (var existingEntity in existingEntities)
            {
                if (mergeEntityConfiguration.KeyConfiguration.KeyProperties != null)
                {
                    var areKeysEqual = mergeEntityConfiguration.KeyConfiguration.UsePrecompiledEqualityComparer
                       ? mergeEntityConfiguration.KeyConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, calculatedEntity)
                       : mergeEntityConfiguration.KeyConfiguration.NaiveEqualityComparer.Equals(existingEntity, calculatedEntity);
                    if (areKeysEqual)
                    {
                        calculatedEntityFoundInExistingEntities = true;
                        break; // don't need to check other existing entities
                    }
                }
            }
            // calculated entity not found in existing entity -> it's an insert
            if (!calculatedEntityFoundInExistingEntities)
            {
                MarkEntityAndPropagateUsingNavigation(mergeEntityConfiguration, calculatedEntity, MergeEntityOperation.Insert); // once an entity is inserted, it's children will also be inserted
                yield return calculatedEntity;
            }
        }
    }

    //private Hashtable InitializeHashtable(MergeEntityConfiguration mergeEntityConfiguration, IEnumerable<object> entities)
    //{
    //    var hashtable = new Hashtable(mergeEntityConfiguration.KeyConfiguration.EqualityComparer);
    //    foreach (var entity in entities)
    //        hashtable.Add(entity, entity);
    //    return hashtable;
    //}

    private bool MergeUsingNavigation(MergeEntityConfiguration mergeEntityConfiguration, object existingEntity, object calculatedEntity)
    {
        var modificationsDetected = false;
        if (mergeEntityConfiguration.NavigationManyConfiguration.NavigationManyProperties != null)
        {
            foreach (var navigationManyProperty in mergeEntityConfiguration.NavigationManyConfiguration.NavigationManyProperties)
                modificationsDetected |= MergeUsingNavigationMany(navigationManyProperty, existingEntity, calculatedEntity);
        }
        if (mergeEntityConfiguration.NavigationOneConfiguration.NavigationOneProperties != null)
        {
            foreach (var navigationOneProperty in mergeEntityConfiguration.NavigationOneConfiguration.NavigationOneProperties)
                modificationsDetected |= MergeUsingNavigationOne(navigationOneProperty, existingEntity, calculatedEntity);
        }
        return modificationsDetected;
    }

    private bool MergeUsingNavigationMany(PropertyInfo navigationProperty, object existingEntity, object calculatedEntity)
    {
        if (navigationProperty == null)
            return false;
        var childType = GetNavigationManyDestinationType(navigationProperty);
        if (childType == null)
            return false;

        if (!Configuration.MergeEntityConfigurations.TryGetValue(childType, out var childMergeEntityConfiguration))
            return false; // TODO: exception

        var existingEntityChildren = navigationProperty.GetValue(existingEntity);
        var calculatedEntityChildren = navigationProperty.GetValue(calculatedEntity);

        // merge children
        var existingChildren = (IEnumerable<object>)existingEntityChildren!;
        var calculatedChildren = (IEnumerable<object>)calculatedEntityChildren!;
        var mergedChildren = Merge(childMergeEntityConfiguration, existingChildren, calculatedChildren);

        // convert children from IEnumerable<object> to List<EnityType>
        var listType = typeof(List<>).MakeGenericType(childType);
        var list = (IList)Activator.CreateInstance(listType)!;
        foreach (var mergedChild in mergedChildren)
            list.Add(mergedChild);
        if (list.Count > 0)
        {
            navigationProperty.SetValue(existingEntity, list);
            return true;
        }
        return false;
    }

    private bool MergeUsingNavigationOne(PropertyInfo navigationProperty, object existingEntity, object calculatedEntity)
    {
        if (navigationProperty == null)
            return false;
        var childType = navigationProperty.PropertyType;
        if (childType == null)
            return false;

        if (!Configuration.MergeEntityConfigurations.TryGetValue(childType, out var childMergeEntityConfiguration))
            return false; // TODO: exception

        var existingEntityChild = navigationProperty.GetValue(existingEntity);
        var calculatedEntityChild = navigationProperty.GetValue(calculatedEntity);

        // was not existing and is now calculated -> it's an insert
        if (existingEntityChild == null && calculatedEntityChild != null)
        {
            navigationProperty.SetValue(existingEntity, calculatedEntityChild);
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
        if (mergeEntityConfiguration.NavigationManyConfiguration.NavigationManyProperties != null)
        {
            foreach (var navigationManyProperty in mergeEntityConfiguration.NavigationManyConfiguration.NavigationManyProperties)
                PropagateUsingNavigationMany(navigationManyProperty, entity, operation);
        }
        if (mergeEntityConfiguration.NavigationOneConfiguration.NavigationOneProperties != null)
        {
            foreach (var navigationOneProperty in mergeEntityConfiguration.NavigationOneConfiguration.NavigationOneProperties)
                PropagateUsingNavigationOne(navigationOneProperty, entity, operation);
        }
    }

    private void PropagateUsingNavigationMany(PropertyInfo navigationProperty, object entity, MergeEntityOperation operation)
    {
        if (navigationProperty == null)
            return;
        var childrenValue = navigationProperty.GetValue(entity);
        if (childrenValue == null)
            return;
        var childType = GetNavigationManyDestinationType(navigationProperty);
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

    private void PropagateUsingNavigationOne(PropertyInfo navigationProperty, object entity, MergeEntityOperation operation)
    {
        if (navigationProperty == null)
            return;
        var childValue = navigationProperty.GetValue(entity);
        if (childValue == null)
            return;
        var childType = navigationProperty.PropertyType;
        if (childType == null)
            return;
        var assignValue = Configuration.MergeEntityConfigurations[childType].MarkAsByOperation[operation];
        if (assignValue != null)
            assignValue.DestinationProperty.SetValue(childValue, assignValue.Value);
    }

    private Type GetNavigationManyDestinationType(PropertyInfo navigationProperty) // TODO: pre calculate this when setting HasMany and HasOne (store in related configuration)
    {
        Type type = navigationProperty.PropertyType;
        // check List<>
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            return type.GetGenericArguments()[0];
        //
        return null; // TODO: throw exception
    }
}
