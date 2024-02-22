using DeepDiff.Comparers;
using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.Extensions;
using DeepDiff.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DeepDiff
{
    internal sealed class DeepDiffEngine
    {
        private IReadOnlyDictionary<Type, EntityConfiguration> EntityConfigurationByTypes { get; }
        private DiffConfigurationBase DiffSingleOrManyConfiguration { get; }

        public DeepDiffEngine(IReadOnlyDictionary<Type, EntityConfiguration> entityConfigurationByTypes, DiffConfigurationBase diffSingleOrManyConfiguration)
        {
            EntityConfigurationByTypes = entityConfigurationByTypes;
            DiffSingleOrManyConfiguration = diffSingleOrManyConfiguration;
        }

        public object InternalDiffSingle(EntityConfiguration entityConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations)
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
            bool areKeysEqual = false;
            if (entityConfiguration.KeyConfiguration.KeyProperties != null)
            {
                areKeysEqual = DiffSingleOrManyConfiguration.UsePrecompiledEqualityComparer && entityConfiguration.KeyConfiguration.UsePrecompiledEqualityComparer
                    ? entityConfiguration.KeyConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, newEntity)
                    : entityConfiguration.KeyConfiguration.NaiveEqualityComparer.Equals(existingEntity, newEntity);
                if (!areKeysEqual) // keys are different -> copy keys
                    entityConfiguration.KeyConfiguration.KeyProperties.CopyPropertyValues(existingEntity, newEntity);
            }

            CompareByPropertyResult compareByPropertyResult = null;
            if (entityConfiguration.ValuesConfiguration?.ValuesProperties != null)
            {
                compareByPropertyResult = DiffSingleOrManyConfiguration.UsePrecompiledEqualityComparer && entityConfiguration.ValuesConfiguration.UsePrecompiledEqualityComparer
                    ? entityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Compare(existingEntity, newEntity)
                    : entityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Compare(existingEntity, newEntity);
            }

            var diffModificationsFound = DiffUsingNavigation(entityConfiguration, existingEntity, newEntity, diffOperations);
            if (!areKeysEqual || (compareByPropertyResult != null && !compareByPropertyResult.IsEqual) || diffModificationsFound) // update
            {
                if (!areKeysEqual || (compareByPropertyResult != null && !compareByPropertyResult.IsEqual) || (diffModificationsFound && DiffSingleOrManyConfiguration.OnUpdateEvenIfModificationsDetectedOnlyInNestedLevel))
                    OnUpdate(entityConfiguration, existingEntity, newEntity, compareByPropertyResult, diffOperations); // new values are different -> copy new values and dump differences
                return existingEntity;
            }

            return null;
        }

        public IList<object> InternalDiffMany(EntityConfiguration entityConfiguration, IEnumerable<object> existingEntities, IEnumerable<object> newEntities, IList<DiffOperationBase> diffOperations)
        {
            var results = new List<object>();

            // no entities to diff
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
                ? InitializeHashtable(entityConfiguration.KeyConfiguration, existingEntities)
                : null;
            var newEntitiesHashtable = CheckIfHashtablesShouldBeUsed(newEntities)
                ? InitializeHashtable(entityConfiguration.KeyConfiguration, newEntities)
                : null;

            // search if every existing entity is found in new entities -> this will detect update and delete
            foreach (var existingEntity in existingEntities)
            {
                var newEntity = newEntitiesHashtable != null
                    ? newEntitiesHashtable[existingEntity]
                    : SearchMatchingEntityByKey(entityConfiguration.KeyConfiguration, newEntities, existingEntity);
                // existing entity found in new entities -> if values are different it's an update
                if (newEntity != null)
                {
                    if (entityConfiguration.ValuesConfiguration?.ValuesProperties != null)
                    {
                        var compareByPropertyResult = DiffSingleOrManyConfiguration.UsePrecompiledEqualityComparer && entityConfiguration.ValuesConfiguration.UsePrecompiledEqualityComparer
                            ? entityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Compare(existingEntity, newEntity)
                            : entityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Compare(existingEntity, newEntity);

                        var diffModificationsFound = DiffUsingNavigation(entityConfiguration, existingEntity, newEntity, diffOperations);
                        if ((compareByPropertyResult != null && !compareByPropertyResult.IsEqual) || diffModificationsFound)
                        {
                            if ((compareByPropertyResult != null && !compareByPropertyResult.IsEqual) || (diffModificationsFound && DiffSingleOrManyConfiguration.OnUpdateEvenIfModificationsDetectedOnlyInNestedLevel))
                                OnUpdate(entityConfiguration, existingEntity, newEntity, compareByPropertyResult, diffOperations); // new values are different -> copy new values and dump differences
                            results.Add(existingEntity);
                        }
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
                    : SearchMatchingEntityByKey(entityConfiguration.KeyConfiguration, existingEntities, newEntity) != null;
                // new entity not found in existing entity -> it's an insert
                if (!newEntityFoundInExistingEntities)
                {
                    OnInsertAndPropagateUsingNavigation(entityConfiguration, newEntity, diffOperations); // once an entity is inserted, it's children will also be inserted
                    results.Add(newEntity);
                }
            }

            return results;
        }

        private bool CheckIfHashtablesShouldBeUsed(IEnumerable<object> existingEntities)
            => DiffSingleOrManyConfiguration.UseHashtable && existingEntities.Count() >= DiffSingleOrManyConfiguration.HashtableThreshold;

        private object SearchMatchingEntityByKey(KeyConfiguration keyConfiguration, IEnumerable<object> entities, object existingEntity)
        {
            foreach (var entity in entities)
            {
                var areKeysEqual = DiffSingleOrManyConfiguration.UsePrecompiledEqualityComparer && keyConfiguration.UsePrecompiledEqualityComparer
                    ? keyConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, entity)
                    : keyConfiguration.NaiveEqualityComparer.Equals(existingEntity, entity);
                if (areKeysEqual)
                    return entity;
            }
            return null!;
        }

        private Hashtable InitializeHashtable(KeyConfiguration keyConfiguration, IEnumerable<object> entities)
        {
            var equalityComparer = DiffSingleOrManyConfiguration.UsePrecompiledEqualityComparer && keyConfiguration.UsePrecompiledEqualityComparer
                ? keyConfiguration.PrecompiledEqualityComparer
                : keyConfiguration.NaiveEqualityComparer;
            var hashtable = new Hashtable(equalityComparer);
            foreach (var entity in entities)
                hashtable.Add(entity, entity);
            return hashtable;
        }

        private bool DiffUsingNavigation(EntityConfiguration entityConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations)
        {
            var modificationsDetected = false;
            if (entityConfiguration.NavigationManyConfigurations != null)
            {
                foreach (var navigationManyConfiguration in entityConfiguration.NavigationManyConfigurations)
                    modificationsDetected |= DiffUsingNavigationMany(navigationManyConfiguration, existingEntity, newEntity, diffOperations);
            }
            if (entityConfiguration.NavigationOneConfigurations != null)
            {
                foreach (var navigationOneConfiguration in entityConfiguration.NavigationOneConfigurations)
                    modificationsDetected |= DiffUsingNavigationOne(navigationOneConfiguration, existingEntity, newEntity, diffOperations);
            }
            return modificationsDetected;
        }

        private bool DiffUsingNavigationMany(NavigationManyConfiguration navigationManyConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations)
        {
            if (navigationManyConfiguration.NavigationProperty == null)
                return false;
            var childType = navigationManyConfiguration.NavigationChildType;
            if (childType == null)
                return false;

            if (!EntityConfigurationByTypes.TryGetValue(childType, out var childEntityConfiguration))
                throw new MissingConfigurationException(childType);

            // get children
            var existingEntityChildren = navigationManyConfiguration.NavigationProperty.GetValue(existingEntity);
            var newEntityChildren = navigationManyConfiguration.NavigationProperty.GetValue(newEntity);

            // diff children
            var existingChildren = (IEnumerable<object>)existingEntityChildren!;
            var newChildren = (IEnumerable<object>)newEntityChildren!;
            var diffChildren = InternalDiffMany(childEntityConfiguration, existingChildren, newChildren, diffOperations);

            // convert diff children from IEnumerable<object> to List<ChildType>
            var listType = typeof(List<>).MakeGenericType(childType);
            var list = (IList)Activator.CreateInstance(listType)!;
            foreach (var diffChild in diffChildren)
                list.Add(diffChild);
            // set navigation many property to diff children
            navigationManyConfiguration.NavigationProperty.SetValue(existingEntity, list);
            //
            if (list.Count > 0)
                return true;
            return false;
        }

        private bool DiffUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations)
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

            // diff child
            var diff = InternalDiffSingle(childEntityConfiguration, existingEntityChild, newEntityChild, diffOperations);

            // set navigation one property to diff child
            navigationOneConfiguration.NavigationProperty.SetValue(existingEntity, diff);

            // not insert/delete/update
            if (diff == null)
                return false;

            return true;
        }

        private void OnUpdate(EntityConfiguration entityConfiguration, object existingEntity, object newEntity, CompareByPropertyResult compareByPropertyResult, IList<DiffOperationBase> diffOperations)
        {
            if (entityConfiguration.UpdateConfiguration != null)
            {
                var updateConfiguration = entityConfiguration.UpdateConfiguration;
                var generateOperations = DiffSingleOrManyConfiguration.GenerateOperations && (entityConfiguration.UpdateConfiguration == null || entityConfiguration.UpdateConfiguration.GenerateOperations);
                // use CopyValues from UpdateConfiguration
                var copyValuesProperties = OnUpdateCopyValues(updateConfiguration, existingEntity, newEntity, generateOperations);
                // use SetValue from UpdateConfiguration
                var setValueProperties = OnUpdateSetValue(updateConfiguration, existingEntity, generateOperations);
                // copy values from ValuesConfiguration, only updated ones
                var updatedProperties = OnUpdateCopyModifiedValues(entityConfiguration, existingEntity, compareByPropertyResult, generateOperations);
                //
                if (generateOperations)
                {
                    var keys = GenerateKeysForOperation(entityConfiguration.KeyConfiguration, existingEntity);
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

        private IReadOnlyCollection<UpdateDiffOperationPropertyInfo> OnUpdateCopyModifiedValues(EntityConfiguration entityConfiguration, object existingEntity, CompareByPropertyResult compareByPropertyResult, bool generateOperations)
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
                    detail.PropertyInfo.SetValue(existingEntity, detail.NewValue);
                }
            }
            return updateDiffOperationPropertyInfos;
        }

        private IReadOnlyCollection<UpdateDiffOperationPropertyInfo> OnUpdateSetValue(UpdateConfiguration updateConfiguration, object existingEntity, bool generateOperations)
        {
            var updateDiffOperationPropertyInfos = new List<UpdateDiffOperationPropertyInfo>();
            if (updateConfiguration.SetValueConfiguration != null)
            {
                if (generateOperations)
                {
                    var existingValue = updateConfiguration.SetValueConfiguration.DestinationProperty.GetValue(existingEntity);
                    var newValue = updateConfiguration.SetValueConfiguration.Value;
                    updateDiffOperationPropertyInfos.Add(new UpdateDiffOperationPropertyInfo
                    {
                        PropertyName = updateConfiguration.SetValueConfiguration.DestinationProperty.Name,
                        ExistingValue = existingValue?.ToString(),
                        NewValue = newValue?.ToString()
                    });
                }
                //
                updateConfiguration.SetValueConfiguration?.DestinationProperty.SetValue(existingEntity, updateConfiguration.SetValueConfiguration.Value);
            }
            return updateDiffOperationPropertyInfos;
        }

        private IReadOnlyCollection<UpdateDiffOperationPropertyInfo> OnUpdateCopyValues(UpdateConfiguration updateConfiguration, object existingEntity, object newEntity, bool generateOperations)
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
                insertConfiguration.SetValueConfiguration?.DestinationProperty.SetValue(newEntity, insertConfiguration.SetValueConfiguration.Value);
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
                deleteConfiguration.SetValueConfiguration?.DestinationProperty.SetValue(existingEntity, deleteConfiguration.SetValueConfiguration.Value);
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
            var childType = navigationManyConfiguration.NavigationChildType;
            if (childType == null)
                return;
            if (!EntityConfigurationByTypes.TryGetValue(childType, out var childEntityConfiguration))
                throw new MissingConfigurationException(childType);
            var children = (IEnumerable<object>)childrenValue;
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

        private void GenerateInsertDiffOperation(EntityConfiguration entityConfiguration, object entity, IList<DiffOperationBase> diffOperations)
        {
            if (DiffSingleOrManyConfiguration.GenerateOperations && (entityConfiguration.InsertConfiguration == null || entityConfiguration.InsertConfiguration.GenerateOperations))
            {
                var keys = GenerateKeysForOperation(entityConfiguration.KeyConfiguration, entity);
                diffOperations.Add(new InsertDiffOperation
                {
                    EntityName = entity.GetType().Name,
                    Keys = keys
                });
            }
        }

        private void GenerateDeleteDiffOperation(EntityConfiguration entityConfiguration, object entity, IList<DiffOperationBase> diffOperations)
        {
            if (DiffSingleOrManyConfiguration.GenerateOperations && (entityConfiguration.DeleteConfiguration == null || entityConfiguration.DeleteConfiguration.GenerateOperations))
            {
                var keys = GenerateKeysForOperation(entityConfiguration.KeyConfiguration, entity);
                diffOperations.Add(new DeleteDiffOperation
                {
                    EntityName = entity.GetType().Name,
                    Keys = keys
                });
            }
        }

        private string GenerateKeysForOperation(KeyConfiguration keyConfiguration, object entity)
        {
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
