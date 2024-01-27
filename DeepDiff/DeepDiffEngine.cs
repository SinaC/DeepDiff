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
        private IReadOnlyDictionary<Type, DiffEntityConfiguration> DiffEntityConfigurationByTypes { get; }
        private DiffSingleOrManyConfigurationBase DiffSingleOrManyConfiguration { get; }

        public DeepDiffEngine(IReadOnlyDictionary<Type, DiffEntityConfiguration> diffEntityConfigurationByTypes, DiffSingleOrManyConfigurationBase diffSingleOrManyConfiguration)
        {
            DiffEntityConfigurationByTypes = diffEntityConfigurationByTypes;
            DiffSingleOrManyConfiguration = diffSingleOrManyConfiguration;
        }

        public object InternalDiffSingle(DiffEntityConfiguration diffEntityConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations)
        {
            // no entity
            if (existingEntity == null && newEntity == null)
                return null;

            // no existing entity -> return new entity as inserted
            if (existingEntity == null)
            {
                OnInsertAndPropagateUsingNavigation(diffEntityConfiguration, newEntity, diffOperations);
                return newEntity;
            }

            // if no new entity -> return existing as deleted
            if (newEntity == null)
            {
                OnDeleteAndPropagateUsingNavigation(diffEntityConfiguration, existingEntity, diffOperations);
                return existingEntity;
            }

            // was existing and is new -> maybe an update
            bool areKeysEqual = false;
            if (diffEntityConfiguration.KeyConfiguration.KeyProperties != null)
            {
                areKeysEqual = DiffSingleOrManyConfiguration.UsePrecompiledEqualityComparer && diffEntityConfiguration.KeyConfiguration.UsePrecompiledEqualityComparer
                    ? diffEntityConfiguration.KeyConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, newEntity)
                    : diffEntityConfiguration.KeyConfiguration.NaiveEqualityComparer.Equals(existingEntity, newEntity);
                if (!areKeysEqual) // keys are different -> copy keys
                    diffEntityConfiguration.KeyConfiguration.KeyProperties.CopyPropertyValues(existingEntity, newEntity);
            }

            bool areNewValuesEquals = false;
            if (diffEntityConfiguration.ValuesConfiguration?.ValuesProperties != null)
            {
                areNewValuesEquals = DiffSingleOrManyConfiguration.UsePrecompiledEqualityComparer && diffEntityConfiguration.ValuesConfiguration.UsePrecompiledEqualityComparer
                    ? diffEntityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, newEntity)
                    : diffEntityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Equals(existingEntity, newEntity);
            }

            var diffModificationsFound = DiffUsingNavigation(diffEntityConfiguration, existingEntity, newEntity, diffOperations);
            if (!areKeysEqual || !areNewValuesEquals || diffModificationsFound) // update
            {
                if (!areKeysEqual || !areNewValuesEquals || (diffModificationsFound && DiffSingleOrManyConfiguration.OnUpdateEvenIfModificationsDetectedOnlyInNestedLevel))
                    OnUpdate(diffEntityConfiguration, existingEntity, newEntity, !areNewValuesEquals, diffOperations); // new values are different -> copy new values and dump differences
                return existingEntity;
            }

            return null;
        }

        public IList<object> InternalDiffMany(DiffEntityConfiguration diffEntityConfiguration, IEnumerable<object> existingEntities, IEnumerable<object> newEntities, IList<DiffOperationBase> diffOperations)
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
                    OnInsertAndPropagateUsingNavigation(diffEntityConfiguration, newEntity, diffOperations); // once an entity is inserted, it's children will also be inserted
                    results.Add(newEntity);
                }
                return results;
            }

            // no new entities -> return existing as deleted
            if (newEntities == null || !newEntities.Any())
            {
                foreach (var existingEntity in existingEntities)
                {
                    OnDeleteAndPropagateUsingNavigation(diffEntityConfiguration, existingEntity, diffOperations); // once an entity is deleted, it's children will also be deleted
                    results.Add(existingEntity);
                }
                return results;
            }

            // we are sure there is at least one existing and one new entity
            var existingEntitiesHashtable = CheckIfHashtablesShouldBeUsed(existingEntities)
                ? InitializeHashtable(diffEntityConfiguration.KeyConfiguration, existingEntities)
                : null;
            var newEntitiesHashtable = CheckIfHashtablesShouldBeUsed(newEntities)
                ? InitializeHashtable(diffEntityConfiguration.KeyConfiguration, newEntities)
                : null;

            // search if every existing entity is found in new entities -> this will detect update and delete
            foreach (var existingEntity in existingEntities)
            {
                var newEntity = newEntitiesHashtable != null
                    ? newEntitiesHashtable[existingEntity]
                    : SearchMatchingEntityByKey(diffEntityConfiguration.KeyConfiguration, newEntities, existingEntity);
                // existing entity found in new entities -> if values are different it's an update
                if (newEntity != null)
                {
                    if (diffEntityConfiguration.ValuesConfiguration?.ValuesProperties != null)
                    {
                        var areNewValuesEquals = DiffSingleOrManyConfiguration.UsePrecompiledEqualityComparer && diffEntityConfiguration.ValuesConfiguration.UsePrecompiledEqualityComparer
                            ? diffEntityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, newEntity)
                            : diffEntityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Equals(existingEntity, newEntity);

                        var diffModificationsFound = DiffUsingNavigation(diffEntityConfiguration, existingEntity, newEntity, diffOperations);
                        if (!areNewValuesEquals || diffModificationsFound)
                        {
                            if (!areNewValuesEquals || (diffModificationsFound && DiffSingleOrManyConfiguration.OnUpdateEvenIfModificationsDetectedOnlyInNestedLevel))
                                OnUpdate(diffEntityConfiguration, existingEntity, newEntity, !areNewValuesEquals, diffOperations); // new values are different -> copy new values and dump differences
                            results.Add(existingEntity);
                        }
                    }
                }
                // existing entity not found in new entities -> it's a delete
                else
                {
                    OnDeleteAndPropagateUsingNavigation(diffEntityConfiguration, existingEntity, diffOperations); // once an entity is deleted, it's children will also be deleted
                    results.Add(existingEntity);
                }
            }

            // search if every new entity is found in existing entities -> this will detect insert
            foreach (var newEntity in newEntities)
            {
                var newEntityFoundInExistingEntities = existingEntitiesHashtable != null
                    ? existingEntitiesHashtable.ContainsKey(newEntity)
                    : SearchMatchingEntityByKey(diffEntityConfiguration.KeyConfiguration, existingEntities, newEntity) != null;
                // new entity not found in existing entity -> it's an insert
                if (!newEntityFoundInExistingEntities)
                {
                    OnInsertAndPropagateUsingNavigation(diffEntityConfiguration, newEntity, diffOperations); // once an entity is inserted, it's children will also be inserted
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

        private bool DiffUsingNavigation(DiffEntityConfiguration diffEntityConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations)
        {
            var modificationsDetected = false;
            if (diffEntityConfiguration.NavigationManyConfigurations != null)
            {
                foreach (var navigationManyConfiguration in diffEntityConfiguration.NavigationManyConfigurations)
                    modificationsDetected |= DiffUsingNavigationMany(navigationManyConfiguration, existingEntity, newEntity, diffOperations);
            }
            if (diffEntityConfiguration.NavigationOneConfigurations != null)
            {
                foreach (var navigationOneConfiguration in diffEntityConfiguration.NavigationOneConfigurations)
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

            if (!DiffEntityConfigurationByTypes.TryGetValue(childType, out var childDiffEntityConfiguration))
                throw new MissingConfigurationException(childType);

            // get children
            var existingEntityChildren = navigationManyConfiguration.NavigationProperty.GetValue(existingEntity);
            var newEntityChildren = navigationManyConfiguration.NavigationProperty.GetValue(newEntity);

            // diff children
            var existingChildren = (IEnumerable<object>)existingEntityChildren!;
            var newChildren = (IEnumerable<object>)newEntityChildren!;
            var diffChildren = InternalDiffMany(childDiffEntityConfiguration, existingChildren, newChildren, diffOperations);

            // convert diff children from IEnumerable<object> to List<ChildType>
            var listType = typeof(List<>).MakeGenericType(childType);
            var list = (IList)Activator.CreateInstance(listType)!;
            foreach (var diffChild in diffChildren)
                list.Add(diffChild);
            // set navigation many property to diff children
            navigationManyConfiguration.NavigationProperty.SetValue(existingEntity, list);
            // if any children, set navigation key
            if (list.Count > 0)
            {
                // copy navigation key if any configured on each children
                if (navigationManyConfiguration.NavigationKeyConfigurations != null)
                {
                    foreach (var child in list)
                        CopyNavigationKeyAndProgagateUsingNavigation(childDiffEntityConfiguration, navigationManyConfiguration, child, existingEntity);
                }
                return true;
            }
            return false;
        }

        private bool DiffUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations)
        {
            if (navigationOneConfiguration.NavigationProperty == null)
                return false;
            var childType = navigationOneConfiguration.NavigationChildType;
            if (childType == null)
                return false;

            if (!DiffEntityConfigurationByTypes.TryGetValue(childType, out var childDiffEntityConfiguration))
                throw new MissingConfigurationException(childType);

            // get child
            var existingEntityChild = navigationOneConfiguration.NavigationProperty.GetValue(existingEntity);
            var newEntityChild = navigationOneConfiguration.NavigationProperty.GetValue(newEntity);

            // diff child
            var diff = InternalDiffSingle(childDiffEntityConfiguration, existingEntityChild, newEntityChild, diffOperations);

            // set navigation one property to diff child
            navigationOneConfiguration.NavigationProperty.SetValue(existingEntity, diff);

            // not insert/delete/update
            if (diff == null)
                return false;

            // if insert, set navigation key
            if (diff == newEntityChild)
            {
                if (navigationOneConfiguration.NavigationKeyConfigurations != null)
                    CopyNavigationKeyAndProgagateUsingNavigation(childDiffEntityConfiguration, navigationOneConfiguration, newEntityChild, existingEntity);
            }

            return true;
        }

        private void CopyNavigationKeyAndProgagateUsingNavigation(DiffEntityConfiguration diffEntityConfiguration, NavigationConfigurationBase navigationConfiguration, object childEntity, object parentEntity)
        {
            if (navigationConfiguration.NavigationKeyConfigurations != null)
            {
                // use NavigationKeyConfigurations to copy foreign keys
                foreach (var navigationKeyConfiguration in navigationConfiguration.NavigationKeyConfigurations)
                {
                    var keyValue = navigationKeyConfiguration.NavigationKeyProperty.GetValue(parentEntity);
                    navigationKeyConfiguration.ChildNavigationKeyProperty.SetValue(childEntity, keyValue);
                }
            }
            PropagateUsingNavigation(diffEntityConfiguration, childEntity, (childDiffEntityConfiguration, parentNavigationConfiguration, child, parent) => CopyNavigationKeyAndProgagateUsingNavigation(childDiffEntityConfiguration, parentNavigationConfiguration, child, parent));
        }

        private void OnUpdate(DiffEntityConfiguration diffEntityConfiguration, object existingEntity, object newEntity, bool copyPropertyValues, IList<DiffOperationBase> diffOperations)
        {
            if (diffEntityConfiguration.UpdateConfiguration != null)
            {
                var updateConfiguration = diffEntityConfiguration.UpdateConfiguration;
                // generate operations
                GenerateUpdateDiffOperations(diffEntityConfiguration, existingEntity, newEntity, diffOperations);
                // use SetValue from UpdateConfiguration
                updateConfiguration.SetValueConfiguration?.DestinationProperty.SetValue(existingEntity, updateConfiguration.SetValueConfiguration.Value);
                // use CopyValues from UpdateConfiguration
                updateConfiguration.CopyValuesConfiguration?.CopyValuesProperties.CopyPropertyValues(existingEntity, newEntity);
                // copy values from ValuesConfiguration
                if (copyPropertyValues)
                    diffEntityConfiguration.ValuesConfiguration.ValuesProperties.CopyPropertyValues(existingEntity, newEntity);
            }
        }

        private void OnInsertAndPropagateUsingNavigation(DiffEntityConfiguration diffEntityConfiguration, object newEntity, IList<DiffOperationBase> diffOperations)
        {
            if (diffEntityConfiguration.InsertConfiguration != null)
            {
                var insertConfiguration = diffEntityConfiguration.InsertConfiguration;
                // generate operations
                GenerateInsertDiffOperation(diffEntityConfiguration, newEntity, diffOperations);
                // use SetValue from InsertConfiguration
                insertConfiguration.SetValueConfiguration?.DestinationProperty.SetValue(newEntity, insertConfiguration.SetValueConfiguration.Value);
            }
            PropagateUsingNavigation(diffEntityConfiguration, newEntity, (childDiffEntityConfiguration, parentNavigationConfiguration, child, parent) => OnInsertAndPropagateUsingNavigation(childDiffEntityConfiguration, child, diffOperations));
        }

        private void OnDeleteAndPropagateUsingNavigation(DiffEntityConfiguration diffEntityConfiguration, object existingEntity, IList<DiffOperationBase> diffOperations)
        {
            if (diffEntityConfiguration.DeleteConfiguration != null)
            {
                var deleteConfiguration = diffEntityConfiguration.DeleteConfiguration;
                // generate options
                GenerateDeleteDiffOperation(diffEntityConfiguration, existingEntity, diffOperations);
                // use SetValue from DeleteConfiguration
                deleteConfiguration.SetValueConfiguration?.DestinationProperty.SetValue(existingEntity, deleteConfiguration.SetValueConfiguration.Value);
            }
            PropagateUsingNavigation(diffEntityConfiguration, existingEntity, (childDiffEntityConfiguration, parentNavigationConfiguration, child, parent) => OnDeleteAndPropagateUsingNavigation(childDiffEntityConfiguration, child, diffOperations));
        }

        private void PropagateUsingNavigation(DiffEntityConfiguration diffEntityConfiguration, object entity, Action<DiffEntityConfiguration, NavigationConfigurationBase, object, object> operation)
        {
            if (diffEntityConfiguration.NavigationManyConfigurations != null)
            {
                foreach (var navigationManyConfiguration in diffEntityConfiguration.NavigationManyConfigurations)
                    PropagateUsingNavigationMany(navigationManyConfiguration, entity, operation);
            }
            if (diffEntityConfiguration.NavigationOneConfigurations != null)
            {
                foreach (var navigationOneConfiguration in diffEntityConfiguration.NavigationOneConfigurations)
                    PropagateUsingNavigationOne(navigationOneConfiguration, entity, operation);
            }
        }

        private void PropagateUsingNavigationMany(NavigationManyConfiguration navigationManyConfiguration, object entity, Action<DiffEntityConfiguration, NavigationConfigurationBase, object, object> operation)
        {
            if (navigationManyConfiguration.NavigationProperty == null)
                return;
            var childrenValue = navigationManyConfiguration.NavigationProperty.GetValue(entity);
            if (childrenValue == null)
                return;
            var childType = navigationManyConfiguration.NavigationChildType;
            if (childType == null)
                return;
            if (!DiffEntityConfigurationByTypes.TryGetValue(childType, out var childDiffEntityConfiguration))
                throw new MissingConfigurationException(childType);
            var children = (IEnumerable<object>)childrenValue;
            foreach (var child in children)
                operation(childDiffEntityConfiguration, navigationManyConfiguration, child, entity);
        }

        private void PropagateUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object entity, Action<DiffEntityConfiguration, NavigationConfigurationBase, object, object> operation)
        {
            if (navigationOneConfiguration.NavigationProperty == null)
                return;
            var child = navigationOneConfiguration.NavigationProperty.GetValue(entity);
            if (child == null)
                return;
            var childType = navigationOneConfiguration.NavigationChildType;
            if (childType == null)
                return;
            if (!DiffEntityConfigurationByTypes.TryGetValue(childType, out var childDiffEntityConfiguration))
                throw new MissingConfigurationException(childType);
            operation(childDiffEntityConfiguration, navigationOneConfiguration, child, entity);
        }

        private void GenerateInsertDiffOperation(DiffEntityConfiguration diffEntityConfiguration, object entity, IList<DiffOperationBase> diffOperations)
        {
            if (DiffSingleOrManyConfiguration.GenerateOperations && (diffEntityConfiguration.InsertConfiguration == null || diffEntityConfiguration.InsertConfiguration.GenerateOperations))
            {
                var keys = GenerateKeysForOperation(diffEntityConfiguration.KeyConfiguration, entity);
                diffOperations.Add(new InsertDiffOperation
                {
                    EntityName = entity.GetType().Name,
                    Keys = keys
                });
            }
        }

        private void GenerateDeleteDiffOperation(DiffEntityConfiguration diffEntityConfiguration, object entity, IList<DiffOperationBase> diffOperations)
        {
            if (DiffSingleOrManyConfiguration.GenerateOperations && (diffEntityConfiguration.DeleteConfiguration == null || diffEntityConfiguration.DeleteConfiguration.GenerateOperations))
            {
                var keys = GenerateKeysForOperation(diffEntityConfiguration.KeyConfiguration, entity);
                diffOperations.Add(new DeleteDiffOperation
                {
                    EntityName = entity.GetType().Name,
                    Keys = keys
                });
            }
        }

        private void GenerateUpdateDiffOperations(DiffEntityConfiguration diffEntityConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations) 
        {
            if (DiffSingleOrManyConfiguration.GenerateOperations && (diffEntityConfiguration.UpdateConfiguration == null || diffEntityConfiguration.UpdateConfiguration.GenerateOperations))
            {
                // Values
                var updatedProperties = new List<UpdateDiffOperationPropertyInfo>();
                if (diffEntityConfiguration.ValuesConfiguration?.ValuesProperties != null)
                {
                    foreach (var propertyInfo in diffEntityConfiguration.ValuesConfiguration.ValuesProperties)
                    {
                        var existingValue = propertyInfo.GetValue(existingEntity);
                        var newValue = propertyInfo.GetValue(newEntity);
                        if (!Equals(existingValue, newValue))
                            updatedProperties.Add(new UpdateDiffOperationPropertyInfo
                            {
                                PropertyName = propertyInfo.Name,
                                ExistingValue = existingValue?.ToString(),
                                NewValue = newValue?.ToString()
                            });
                    }
                }
                // SetValue
                var setValueProperties = new List<UpdateDiffOperationPropertyInfo>();
                if (diffEntityConfiguration.UpdateConfiguration?.SetValueConfiguration != null)
                {
                    var existingValue = diffEntityConfiguration.UpdateConfiguration.SetValueConfiguration.DestinationProperty.GetValue(existingEntity);
                    var newValue = diffEntityConfiguration.UpdateConfiguration.SetValueConfiguration.Value;
                    if (!Equals(existingValue, newValue))
                        setValueProperties.Add(new UpdateDiffOperationPropertyInfo
                        {
                            PropertyName = diffEntityConfiguration.UpdateConfiguration.SetValueConfiguration.DestinationProperty.Name,
                            ExistingValue = existingValue?.ToString(),
                            NewValue = newValue?.ToString()
                        });
                }
                // CopyValues
                var copyValuesProperties = new List<UpdateDiffOperationPropertyInfo>();
                if (diffEntityConfiguration.UpdateConfiguration?.CopyValuesConfiguration != null)
                {
                    foreach (var propertyInfo in diffEntityConfiguration.UpdateConfiguration.CopyValuesConfiguration.CopyValuesProperties)
                    {
                        var existingValue = propertyInfo.GetValue(existingEntity);
                        var newValue = propertyInfo.GetValue(newEntity);
                        if (!Equals(existingValue, newValue))
                            copyValuesProperties.Add(new UpdateDiffOperationPropertyInfo
                            {
                                PropertyName = propertyInfo.Name,
                                ExistingValue = existingValue?.ToString(),
                                NewValue = newValue?.ToString()
                            });
                    }
                }
                //
                if (updatedProperties.Any() || setValueProperties.Any() || copyValuesProperties.Any())
                {
                    var keys = GenerateKeysForOperation(diffEntityConfiguration.KeyConfiguration, existingEntity);
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
