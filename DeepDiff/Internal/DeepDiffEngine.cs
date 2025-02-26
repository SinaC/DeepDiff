﻿using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.Internal.Comparers;
using DeepDiff.Internal.Configuration;
using DeepDiff.Internal.Extensions;
using DeepDiff.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DeepDiff.Internal
{
    internal sealed class DeepDiffEngine
    {
        private IReadOnlyDictionary<Type, EntityConfiguration> EntityConfigurationByTypes { get; }
        private DiffEngineConfiguration DiffEngineConfiguration { get; }

        public DeepDiffEngine(IReadOnlyDictionary<Type, EntityConfiguration> entityConfigurationByTypes, DiffEngineConfiguration diffEngineConfiguration)
        {
            EntityConfigurationByTypes = entityConfigurationByTypes;
            DiffEngineConfiguration = diffEngineConfiguration;
        }

        public object InternalMergeSingle(EntityConfiguration entityConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations)
        {
            // no entity
            if (existingEntity == null && newEntity == null)
                return null;

            // no existing entity -> return new entity as inserted
            if (existingEntity == null)
            {
                OnInsertAndPropagateUsingNavigation(entityConfiguration, newEntity, diffOperations);
                return newEntity;
            }

            // if no new entity -> return existing as deleted
            if (newEntity == null)
            {
                OnDeleteAndPropagateUsingNavigation(entityConfiguration, existingEntity, diffOperations);
                return existingEntity;
            }

            // was existing and is new -> maybe an update
            bool areKeysEqual = true;
            if (!entityConfiguration.NoKey && entityConfiguration.KeyConfiguration.KeyProperties != null)
            {
                areKeysEqual = DiffEngineConfiguration.UsePrecompiledEqualityComparer
                    ? entityConfiguration.KeyConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, newEntity)
                    : entityConfiguration.KeyConfiguration.NaiveEqualityComparer.Equals(existingEntity, newEntity);
                if (!areKeysEqual && !DiffEngineConfiguration.GenerateOperationsOnly) // keys are different -> copy keys
                    entityConfiguration.KeyConfiguration.KeyProperties.CopyPropertyValues(existingEntity, newEntity);
            }

            // compare values
            CompareByPropertyResult compareByPropertyResult = null;
            if (entityConfiguration.ValuesConfiguration?.ValuesProperties != null)
            {
                compareByPropertyResult = DiffEngineConfiguration.UsePrecompiledEqualityComparer
                    ? entityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Compare(existingEntity, newEntity)
                    : entityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Compare(existingEntity, newEntity);
            }

            // perform merge on nested entities
            var diffModificationsFound = MergeUsingNavigation(entityConfiguration, existingEntity, newEntity, diffOperations);

            // check force update if equals
            var forceOnUpdate = CheckIfOnUpdateHasToBeForced(entityConfiguration, existingEntity);

            //
            if (!areKeysEqual || compareByPropertyResult != null && !compareByPropertyResult.IsEqual || diffModificationsFound || forceOnUpdate) // update
            {
                if (!areKeysEqual || compareByPropertyResult != null && !compareByPropertyResult.IsEqual || diffModificationsFound && (DiffEngineConfiguration.ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel || entityConfiguration.ForceUpdateIfConfiguration?.NestedEntitiesModifiedEnabled == true) || forceOnUpdate)
                    OnUpdate(entityConfiguration, existingEntity, newEntity, compareByPropertyResult, diffOperations);
                return existingEntity;
            }

            return null;
        }

        public List<object> InternalMergeMany(EntityConfiguration entityConfiguration, IEnumerable<object> existingEntities, IEnumerable<object> newEntities, IList<DiffOperationBase> diffOperations)
        {
            if (entityConfiguration.NoKey)
                throw new NoKeyEntityInDiffManyException(entityConfiguration.EntityType);

            var results = new List<object>();

            // no entities to merge
            if ((existingEntities == null || !existingEntities.Any()) && (newEntities == null || !newEntities.Any()))
                return results;

            // no existing entities -> return new as inserted
            if (existingEntities == null || !existingEntities.Any())
            {
                foreach (var newEntity in newEntities)
                {
                    OnInsertAndPropagateUsingNavigation(entityConfiguration, newEntity, diffOperations); // once an entity is inserted, it's children will also be inserted
                    results.Add(newEntity);
                }
                return results;
            }

            // no new entities -> return existing as deleted
            if (newEntities == null || !newEntities.Any())
            {
                foreach (var existingEntity in existingEntities)
                {
                    OnDeleteAndPropagateUsingNavigation(entityConfiguration, existingEntity, diffOperations); // once an entity is deleted, it's children will also be deleted
                    results.Add(existingEntity);
                }
                return results;
            }

            // we are sure there is at least one existing and one new entity
            var existingEntitiesHashtable = CheckIfHashtablesShouldBeUsed(existingEntities)
                ? InitializeHashtable(entityConfiguration, entityConfiguration.KeyConfiguration, existingEntities)
                : null;
            var newEntitiesHashtable = CheckIfHashtablesShouldBeUsed(newEntities)
                ? InitializeHashtable(entityConfiguration, entityConfiguration.KeyConfiguration, newEntities)
                : null;

            // search if every existing entity is found in new entities -> this will detect update and delete
            foreach (var existingEntity in existingEntities)
            {
                var newEntity = newEntitiesHashtable != null
                    ? newEntitiesHashtable[existingEntity]
                    : SearchMatchingEntityByKey(entityConfiguration, entityConfiguration.KeyConfiguration, newEntities, existingEntity);
                // existing entity found in new entities -> maybe an update
                if (newEntity != null)
                {
                    // compare values
                    CompareByPropertyResult compareByPropertyResult = null;
                    if (entityConfiguration.ValuesConfiguration?.ValuesProperties != null)
                    {
                        compareByPropertyResult = DiffEngineConfiguration.UsePrecompiledEqualityComparer
                            ? entityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Compare(existingEntity, newEntity)
                            : entityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Compare(existingEntity, newEntity);
                    }

                    // perform merge on nested entities
                    var diffModificationsFound = MergeUsingNavigation(entityConfiguration, existingEntity, newEntity, diffOperations);

                    // check force update if equals
                    var forceOnUpdate = CheckIfOnUpdateHasToBeForced(entityConfiguration, existingEntity);

                    //
                    if (compareByPropertyResult != null && !compareByPropertyResult.IsEqual || diffModificationsFound || forceOnUpdate) // update
                    {
                        if (compareByPropertyResult != null && !compareByPropertyResult.IsEqual || diffModificationsFound && (DiffEngineConfiguration.ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel || entityConfiguration.ForceUpdateIfConfiguration?.NestedEntitiesModifiedEnabled == true) || forceOnUpdate)
                            OnUpdate(entityConfiguration, existingEntity, newEntity, compareByPropertyResult, diffOperations);
                        results.Add(existingEntity);
                    }
                }
                // existing entity not found in new entities -> it's a delete
                else
                {
                    OnDeleteAndPropagateUsingNavigation(entityConfiguration, existingEntity, diffOperations); // once an entity is deleted, it's children will also be deleted
                    results.Add(existingEntity);
                }
            }

            // search if every new entity is found in existing entities -> this will detect insert
            foreach (var newEntity in newEntities)
            {
                var newEntityFoundInExistingEntities = existingEntitiesHashtable != null
                    ? existingEntitiesHashtable.ContainsKey(newEntity)
                    : SearchMatchingEntityByKey(entityConfiguration, entityConfiguration.KeyConfiguration, existingEntities, newEntity) != null;
                // new entity not found in existing entity -> it's an insert
                if (!newEntityFoundInExistingEntities)
                {
                    OnInsertAndPropagateUsingNavigation(entityConfiguration, newEntity, diffOperations); // once an entity is inserted, it's children will also be inserted
                    results.Add(newEntity);
                }
            }

            return results;
        }

        private List<object> InternalMergeManyWithDerivedTypes(IEnumerable<object> existingEntities, IEnumerable<object> newEntities, IList<DiffOperationBase> diffOperations)
        {
            // perform multiple merge, one by unique type found in existing and new entities collection
            var existingEntitiesByTypes = existingEntities?.ToLookup(x => x.GetType()) ?? EmptyLookup<Type, object>.Instance;
            var newEntitiesByTypes = newEntities?.ToLookup(x => x.GetType()) ?? EmptyLookup<Type, object>.Instance;

            var results = new List<object>();
            foreach (var entityType in existingEntitiesByTypes.Select(x => x.Key).Union(newEntitiesByTypes.Select(x => x.Key)).Distinct())
            {
                if (!EntityConfigurationByTypes.TryGetValue(entityType, out var entityConfiguration))
                    throw new MissingConfigurationException(entityType);
                var existingEntitiesByType = existingEntitiesByTypes?[entityType];
                var newEntitiesByType = newEntitiesByTypes?[entityType];
                var subResults = InternalMergeMany(entityConfiguration, existingEntitiesByType, newEntitiesByType, diffOperations);
                results.AddRange(subResults);
            }

            return results;
        }

        private bool CheckIfHashtablesShouldBeUsed(IEnumerable<object> existingEntities)
            => DiffEngineConfiguration.UseHashtable && existingEntities.Count() >= DiffEngineConfiguration.HashtableThreshold;

        private object SearchMatchingEntityByKey(EntityConfiguration entityConfiguration, KeyConfiguration keyConfiguration, IEnumerable<object> entities, object existingEntity)
        {
            var comparer = DiffEngineConfiguration.UsePrecompiledEqualityComparer
                    ? keyConfiguration.PrecompiledEqualityComparer
                    : keyConfiguration.NaiveEqualityComparer;
            try
            {
                return entities.SingleOrDefault(x => comparer.Equals(x, existingEntity));
            }
            catch(InvalidOperationException)
            {
                var keys = GenerateKeysForException(entityConfiguration, keyConfiguration, existingEntity);
                throw new DuplicateKeysException(entityConfiguration.EntityType, keys);
            }
        }

        private Hashtable InitializeHashtable(EntityConfiguration entityConfiguration, KeyConfiguration keyConfiguration, IEnumerable<object> entities)
        {
            var equalityComparer = DiffEngineConfiguration.UsePrecompiledEqualityComparer
                ? keyConfiguration.PrecompiledEqualityComparer
                : keyConfiguration.NaiveEqualityComparer;
            var hashtable = new Hashtable(equalityComparer);
            foreach (var entity in entities)
            {
                try
                {
                    hashtable.Add(entity, entity);
                }
                catch (ArgumentException)
                {
                    var keys = GenerateKeysForException(entityConfiguration, keyConfiguration, entity);
                    throw new DuplicateKeysException(entityConfiguration.EntityType, keys);
                }
            }
            return hashtable;
        }

        private bool MergeUsingNavigation(EntityConfiguration entityConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations)
        {
            var modificationsDetected = false;
            if (entityConfiguration.NavigationManyConfigurations != null)
            {
                foreach (var navigationManyConfiguration in entityConfiguration.NavigationManyConfigurations)
                    modificationsDetected |= MergeUsingNavigationMany(navigationManyConfiguration, existingEntity, newEntity, diffOperations);
            }
            if (entityConfiguration.NavigationOneConfigurations != null)
            {
                foreach (var navigationOneConfiguration in entityConfiguration.NavigationOneConfigurations)
                    modificationsDetected |= MergeUsingNavigationOne(navigationOneConfiguration, existingEntity, newEntity, diffOperations);
            }
            return modificationsDetected;
        }

        private bool MergeUsingNavigationMany(NavigationManyConfiguration navigationManyConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations)
        {
            if (navigationManyConfiguration.NavigationProperty == null)
                return false;
            var childType = navigationManyConfiguration.NavigationChildType;
            if (childType == null)
                return false;

            // get children
            var existingEntityChildren = navigationManyConfiguration.NavigationProperty.GetValue(existingEntity);
            var newEntityChildren = navigationManyConfiguration.NavigationProperty.GetValue(newEntity);

            // merge children
            var existingChildren = (IEnumerable<object>)existingEntityChildren!;
            var newChildren = (IEnumerable<object>)newEntityChildren!;
            IList<object> mergedChildren;
            if (navigationManyConfiguration.UseDerivedTypes)
                mergedChildren = InternalMergeManyWithDerivedTypes(existingChildren, newChildren, diffOperations);
            else
            {
                if (!EntityConfigurationByTypes.TryGetValue(childType, out var childEntityConfiguration))
                    throw new MissingConfigurationException(childType);
                mergedChildren = InternalMergeMany(childEntityConfiguration, existingChildren, newChildren, diffOperations);
            }

            // convert merged children from IEnumerable<object> to List<ChildType>
            var listType = typeof(List<>).MakeGenericType(childType);
            var list = (IList)Activator.CreateInstance(listType)!;
            foreach (var mergedChild in mergedChildren)
                list.Add(mergedChild);
            // set navigation many property to merged children
            if (!DiffEngineConfiguration.GenerateOperationsOnly)
                navigationManyConfiguration.NavigationProperty.SetValue(existingEntity, list);
            //
            if (list.Count > 0)
                return true;
            return false;
        }

        private bool MergeUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations)
        {
            if (navigationOneConfiguration.NavigationProperty == null)
                return false;
            var childType = navigationOneConfiguration.NavigationChildType;
            if (childType == null)
                return false;

            if (!EntityConfigurationByTypes.TryGetValue(childType, out var childEntityConfiguration))
                throw new MissingConfigurationException(childType);

            // get child
            var existingEntityChild = navigationOneConfiguration.NavigationProperty.GetValue(existingEntity);
            var newEntityChild = navigationOneConfiguration.NavigationProperty.GetValue(newEntity);

            // merge child
            var mergedChild = InternalMergeSingle(childEntityConfiguration, existingEntityChild, newEntityChild, diffOperations);

            // set navigation one property to merged child
            if (!DiffEngineConfiguration.GenerateOperationsOnly)
                navigationOneConfiguration.NavigationProperty.SetValue(existingEntity, mergedChild);

            // not insert/delete/update
            if (mergedChild == null)
                return false;

            return true;
        }

        private void OnUpdate(EntityConfiguration entityConfiguration, object existingEntity, object newEntity, CompareByPropertyResult compareByPropertyResult, IList<DiffOperationBase> diffOperations)
        {
            if (entityConfiguration.UpdateConfiguration != null)
            {
                var updateConfiguration = entityConfiguration.UpdateConfiguration;
                var generateOperations = DiffEngineConfiguration.OperationsToGenerate.HasFlag(DiffOperations.UpdateValue) || DiffEngineConfiguration.OperationsToGenerate.HasFlag(DiffOperations.UpdateSetValue) || DiffEngineConfiguration.OperationsToGenerate.HasFlag(DiffOperations.UpdateCopyValue);
                // use CopyValues from UpdateConfiguration
                var copyValuesProperties = OnUpdateCopyValues(updateConfiguration, existingEntity, newEntity, DiffEngineConfiguration.OperationsToGenerate.HasFlag(DiffOperations.UpdateCopyValue));
                // use SetValue from UpdateConfiguration
                var setValueProperties = OnUpdateSetValue(updateConfiguration, existingEntity, DiffEngineConfiguration.OperationsToGenerate.HasFlag(DiffOperations.UpdateSetValue));
                // copy values from ValuesConfiguration, only updated ones
                var updatedProperties = OnUpdateCopyModifiedValues(entityConfiguration, existingEntity, compareByPropertyResult, DiffEngineConfiguration.OperationsToGenerate.HasFlag(DiffOperations.UpdateValue));
                //
                if (generateOperations)
                {
                    var keys = GenerateKeysForOperation(entityConfiguration, entityConfiguration.KeyConfiguration, existingEntity);
                    diffOperations.Add(new UpdateDiffOperation
                    {
                        Keys = keys,
                        EntityName = existingEntity.GetType().Name,
                        UpdatedProperties = updatedProperties,
                        SetValueProperties = setValueProperties,
                        CopyValuesProperties = copyValuesProperties
                    });
                }
            }
        }

        private List<UpdateDiffOperationPropertyInfo> OnUpdateCopyModifiedValues(EntityConfiguration entityConfiguration, object existingEntity, CompareByPropertyResult compareByPropertyResult, bool generateOperations)
        {
            var updateDiffOperationPropertyInfos = new List<UpdateDiffOperationPropertyInfo>();
            if (compareByPropertyResult != null && !compareByPropertyResult.IsEqual)
            {
                foreach (var detail in compareByPropertyResult.Details.Where(x => entityConfiguration.ValuesConfiguration.ValuesProperties.Contains(x.PropertyInfo))) // copy modified properties found in values configuration (modified properties will always be a subset of values but we are testing to be sure)
                {
                    if (generateOperations)
                    {
                        updateDiffOperationPropertyInfos.Add(new UpdateDiffOperationPropertyInfo
                        {
                            PropertyName = detail.PropertyInfo.Name,
                            ExistingValue = detail.OldValue?.ToString(),
                            NewValue = detail.NewValue?.ToString()
                        });
                    }
                    //
                    if (!DiffEngineConfiguration.GenerateOperationsOnly)
                        detail.PropertyInfo.SetValue(existingEntity, detail.NewValue);
                }
            }
            return updateDiffOperationPropertyInfos;
        }

        private List<UpdateDiffOperationPropertyInfo> OnUpdateSetValue(UpdateConfiguration updateConfiguration, object existingEntity, bool generateOperations)
        {
            var updateDiffOperationPropertyInfos = new List<UpdateDiffOperationPropertyInfo>();
            if (updateConfiguration.SetValueConfigurations != null && updateConfiguration.SetValueConfigurations.Count > 0)
            {
                foreach (var setValueConfiguration in updateConfiguration.SetValueConfigurations)
                {
                    if (generateOperations)
                    {
                        var existingValue = setValueConfiguration.DestinationProperty.GetValue(existingEntity);
                        var newValue = setValueConfiguration.Value;
                        updateDiffOperationPropertyInfos.Add(new UpdateDiffOperationPropertyInfo
                        {
                            PropertyName = setValueConfiguration.DestinationProperty.Name,
                            ExistingValue = existingValue?.ToString(),
                            NewValue = newValue?.ToString()
                        });
                    }
                    //
                    if (!DiffEngineConfiguration.GenerateOperationsOnly)
                        setValueConfiguration?.DestinationProperty.SetValue(existingEntity, setValueConfiguration.Value);
                }
            }
            return updateDiffOperationPropertyInfos;
        }

        private List<UpdateDiffOperationPropertyInfo> OnUpdateCopyValues(UpdateConfiguration updateConfiguration, object existingEntity, object newEntity, bool generateOperations)
        {
            var updateDiffOperationPropertyInfos = new List<UpdateDiffOperationPropertyInfo>();
            if (updateConfiguration.CopyValuesConfiguration != null)
            {
                if (generateOperations)
                {
                    foreach (var propertyInfo in updateConfiguration.CopyValuesConfiguration.CopyValuesProperties)
                    {
                        var existingValue = propertyInfo.GetValue(existingEntity);
                        var newValue = propertyInfo.GetValue(newEntity);
                        updateDiffOperationPropertyInfos.Add(new UpdateDiffOperationPropertyInfo
                        {
                            PropertyName = propertyInfo.Name,
                            ExistingValue = existingValue?.ToString(),
                            NewValue = newValue?.ToString()
                        });
                    }
                }
                //
                if (!DiffEngineConfiguration.GenerateOperationsOnly)
                    updateConfiguration.CopyValuesConfiguration?.CopyValuesProperties.CopyPropertyValues(existingEntity, newEntity);
            }
            return updateDiffOperationPropertyInfos;
        }

        private void OnInsertAndPropagateUsingNavigation(EntityConfiguration entityConfiguration, object newEntity, IList<DiffOperationBase> diffOperations)
        {
            if (entityConfiguration.InsertConfiguration != null)
            {
                var insertConfiguration = entityConfiguration.InsertConfiguration;
                // generate operations
                GenerateInsertDiffOperation(entityConfiguration, newEntity, diffOperations);
                // use SetValue from InsertConfiguration
                if (insertConfiguration.SetValueConfigurations != null && insertConfiguration.SetValueConfigurations.Count > 0 && !DiffEngineConfiguration.GenerateOperationsOnly)
                {
                    foreach(var setValueConfiguration in insertConfiguration.SetValueConfigurations)
                        setValueConfiguration.DestinationProperty.SetValue(newEntity, setValueConfiguration.Value);
                }
            }

            PropagateUsingNavigation(entityConfiguration, newEntity, (childEntityConfiguration, parentNavigationConfiguration, child, parent) => OnInsertAndPropagateUsingNavigation(childEntityConfiguration, child, diffOperations));
        }

        private void OnDeleteAndPropagateUsingNavigation(EntityConfiguration entityConfiguration, object existingEntity, IList<DiffOperationBase> diffOperations)
        {
            if (entityConfiguration.DeleteConfiguration != null)
            {
                var deleteConfiguration = entityConfiguration.DeleteConfiguration;
                // generate options
                GenerateDeleteDiffOperation(entityConfiguration, existingEntity, diffOperations);
                // use SetValue from DeleteConfiguration
                if (deleteConfiguration.SetValueConfigurations != null && deleteConfiguration.SetValueConfigurations.Count > 0 && !DiffEngineConfiguration.GenerateOperationsOnly)
                {
                    foreach (var setValueConfiguration in deleteConfiguration.SetValueConfigurations)
                        setValueConfiguration.DestinationProperty.SetValue(existingEntity, setValueConfiguration.Value);
                }
                
            }
            PropagateUsingNavigation(entityConfiguration, existingEntity, (childEntityConfiguration, parentNavigationConfiguration, child, parent) => OnDeleteAndPropagateUsingNavigation(childEntityConfiguration, child, diffOperations));
        }

        private void PropagateUsingNavigation(EntityConfiguration entityConfiguration, object entity, Action<EntityConfiguration, NavigationConfigurationBase, object, object> operation)
        {
            if (entityConfiguration.NavigationManyConfigurations != null)
            {
                foreach (var navigationManyConfiguration in entityConfiguration.NavigationManyConfigurations)
                    PropagateUsingNavigationMany(navigationManyConfiguration, entity, operation);
            }
            if (entityConfiguration.NavigationOneConfigurations != null)
            {
                foreach (var navigationOneConfiguration in entityConfiguration.NavigationOneConfigurations)
                    PropagateUsingNavigationOne(navigationOneConfiguration, entity, operation);
            }
        }

        private void PropagateUsingNavigationMany(NavigationManyConfiguration navigationManyConfiguration, object entity, Action<EntityConfiguration, NavigationConfigurationBase, object, object> operation)
        {
            if (navigationManyConfiguration.NavigationProperty == null)
                return;
            var childrenValue = navigationManyConfiguration.NavigationProperty.GetValue(entity);
            if (childrenValue == null)
                return;
            var children = (IEnumerable<object>)childrenValue;
            if (navigationManyConfiguration.UseDerivedTypes) // when derived types must be used, children must be grouped by type then type from each group must be used instead of collection child type
            {
                var childrenByTypes = children?.ToLookup(x => x.GetType()) ?? EmptyLookup<Type, object>.Instance;
                foreach (var childrenByType in childrenByTypes)
                {
                    var childType = childrenByType.Key;
                    PropagateUsingNavigationMany(navigationManyConfiguration, entity, childType, childrenByType, operation);
                }
            }
            else
            {
                var childType = navigationManyConfiguration.NavigationChildType;
                PropagateUsingNavigationMany(navigationManyConfiguration, entity, childType, children, operation);
            }
        }

        private void PropagateUsingNavigationMany(NavigationManyConfiguration navigationManyConfiguration, object entity, Type childType, IEnumerable<object> children, Action<EntityConfiguration, NavigationConfigurationBase, object, object> operation)
        {
            if (!EntityConfigurationByTypes.TryGetValue(childType, out var childEntityConfiguration))
                throw new MissingConfigurationException(childType);
            foreach (var child in children)
                operation(childEntityConfiguration, navigationManyConfiguration, child, entity);
        }

        private void PropagateUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object entity, Action<EntityConfiguration, NavigationConfigurationBase, object, object> operation)
        {
            if (navigationOneConfiguration.NavigationProperty == null)
                return;
            var child = navigationOneConfiguration.NavigationProperty.GetValue(entity);
            if (child == null)
                return;
            var childType = navigationOneConfiguration.NavigationChildType;
            if (childType == null)
                return;
            if (!EntityConfigurationByTypes.TryGetValue(childType, out var childEntityConfiguration))
                throw new MissingConfigurationException(childType);
            operation(childEntityConfiguration, navigationOneConfiguration, child, entity);
        }

        private static bool CheckIfOnUpdateHasToBeForced(EntityConfiguration entityConfiguration, object entity)
        {
            if (entityConfiguration.ForceUpdateIfConfiguration?.ForceUpdateIfEqualsConfigurations != null)
            {
                foreach (var forceUpdateIfEqualsConfiguration in entityConfiguration.ForceUpdateIfConfiguration?.ForceUpdateIfEqualsConfigurations)
                {
                    var value = forceUpdateIfEqualsConfiguration.CompareToProperty.GetValue(entity);
                    if (value.Equals(forceUpdateIfEqualsConfiguration.CompareToValue))
                        return true;
                }
            }
            return false;
        }

        private void GenerateInsertDiffOperation(EntityConfiguration entityConfiguration, object entity, IList<DiffOperationBase> diffOperations)
        {
            if (DiffEngineConfiguration.OperationsToGenerate.HasFlag(DiffOperations.Insert))
            {
                var keys = GenerateKeysForOperation(entityConfiguration, entityConfiguration.KeyConfiguration, entity);
                diffOperations.Add(new InsertDiffOperation
                {
                    EntityName = entity.GetType().Name,
                    Keys = keys
                });
            }
        }

        private void GenerateDeleteDiffOperation(EntityConfiguration entityConfiguration, object entity, IList<DiffOperationBase> diffOperations)
        {
            if (DiffEngineConfiguration.OperationsToGenerate.HasFlag(DiffOperations.Delete))
            {
                var keys = GenerateKeysForOperation(entityConfiguration, entityConfiguration.KeyConfiguration, entity);
                diffOperations.Add(new DeleteDiffOperation
                {
                    EntityName = entity.GetType().Name,
                    Keys = keys
                });
            }
        }

        private static Dictionary<string, string> GenerateKeysForOperation(EntityConfiguration entityConfiguration, KeyConfiguration keyConfiguration, object entity)
        {
            if (entityConfiguration.NoKey)
                return null;

            var result = new Dictionary<string, string>();
            foreach (var propertyInfo in keyConfiguration.KeyProperties)
            {
                var key = propertyInfo.GetValue(entity);
                result.Add(propertyInfo.Name, key?.ToString());
            }
            return result;
        }

        private static string GenerateKeysForException(EntityConfiguration entityConfiguration, KeyConfiguration keyConfiguration, object entity)
        {
            if (entityConfiguration.NoKey)
                return string.Empty;

            var keys = new List<string>();
            foreach (var propertyInfo in keyConfiguration.KeyProperties)
            {
                var key = propertyInfo.GetValue(entity);
                keys.Add(key?.ToString());
            }
            return string.Join(",", keys);
        }
    }
}
