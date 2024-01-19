using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.Extensions;
using DeepDiff.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DeepDiff
{
    internal sealed class DeepDiffEngine
    {
        private IReadOnlyDictionary<Type, DiffEntityConfiguration> DiffEntityConfigurationByTypes { get; }
        private DiffSingleOrManyConfiguration DiffSingleOrManyConfiguration { get; }

        private const int ElapsedHashtableCreation = 0;
        private const int ElapsedSearchEntity = 1;
        private const int ElapsedConvertToList = 1;
        private Stopwatch Stopwatch { get; }
        internal long[] Elapsed { get; } = new long[3];

        public DeepDiffEngine(IReadOnlyDictionary<Type, DiffEntityConfiguration> diffEntityConfigurationByTypes, DiffSingleOrManyConfiguration diffSingleOrManyConfiguration)
        {
            DiffEntityConfigurationByTypes = diffEntityConfigurationByTypes;
            DiffSingleOrManyConfiguration = diffSingleOrManyConfiguration;

            Stopwatch = new Stopwatch();
        }

        public object InternalDiffSingle(DiffEntityConfiguration diffEntityConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations)
        {

            // no entity
            if (existingEntity == null && newEntity == null)
                return null;

            // no existing entity -> return new entity as inserted
            if (existingEntity == null)
            {
                OnInsertAndPropagateUsingNavigation(diffEntityConfiguration, newEntity);
                return newEntity;
            }

            // if no new entity -> return existing as deleted
            if (newEntity == null)
            {
                OnDeleteAndPropagateUsingNavigation(diffEntityConfiguration, existingEntity);
                return existingEntity;
            }

            // was existing and is new -> maybe an update
            bool areKeysEqual = false;
            if (diffEntityConfiguration.KeyConfiguration.KeyProperties != null)
            {
                areKeysEqual = diffEntityConfiguration.KeyConfiguration.UsePrecompiledEqualityComparer
                    ? diffEntityConfiguration.KeyConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, newEntity)
                    : diffEntityConfiguration.KeyConfiguration.NaiveEqualityComparer.Equals(existingEntity, newEntity);
                if (!areKeysEqual) // keys are different -> copy keys
                    diffEntityConfiguration.KeyConfiguration.KeyProperties.CopyPropertyValues(existingEntity, newEntity);
            }

            bool areNewValuesEquals = false;
            if (diffEntityConfiguration.ValuesConfiguration?.ValuesProperties != null)
            {
                areNewValuesEquals = diffEntityConfiguration.ValuesConfiguration.UsePrecompiledEqualityComparer
                    ? diffEntityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, newEntity)
                    : diffEntityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Equals(existingEntity, newEntity);
                // new values are different -> copy new values
                if (!areNewValuesEquals)
                    diffEntityConfiguration.ValuesConfiguration.ValuesProperties.CopyPropertyValues(existingEntity, newEntity);
            }

            var diffModificationsFound = DiffUsingNavigation(diffEntityConfiguration, existingEntity, newEntity, diffOperations);
            if (!areKeysEqual || !areNewValuesEquals || diffModificationsFound) // update
            {
                if (!areKeysEqual || !areNewValuesEquals || (diffModificationsFound && DiffSingleOrManyConfiguration.OnUpdateEvenIfModificationsDetectedOnlyInNestedLevel))
                    OnUpdate(diffEntityConfiguration, existingEntity, newEntity);
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
                    OnInsertAndPropagateUsingNavigation(diffEntityConfiguration, newEntity); // once an entity is inserted, it's children will also be inserted
                    results.Add(newEntity);
                }
                return results;
            }

            // no new entities -> return existing as deleted
            if (newEntities == null || !newEntities.Any())
            {
                foreach (var existingEntity in existingEntities)
                {
                    OnDeleteAndPropagateUsingNavigation(diffEntityConfiguration, existingEntity); // once an entity is deleted, it's children will also be deleted
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
                Stopwatch.Restart();
                var newEntity = newEntitiesHashtable != null
                    ? newEntitiesHashtable[existingEntity]
                    : SearchMatchingEntityByKey(diffEntityConfiguration.KeyConfiguration, newEntities, existingEntity);
                Stopwatch.Stop();
                Elapsed[ElapsedSearchEntity] += Stopwatch.ElapsedMilliseconds;
                // existing entity found in new entities -> if values are different it's an update
                if (newEntity != null)
                {
                    if (diffEntityConfiguration.ValuesConfiguration?.ValuesProperties != null)
                    {
                        var areNewValuesEquals = diffEntityConfiguration.ValuesConfiguration.UsePrecompiledEqualityComparer
                            ? diffEntityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, newEntity)
                            : diffEntityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Equals(existingEntity, newEntity);
                        // new values are different -> copy new values and dump differences
                        if (!areNewValuesEquals)
                        {
                            GenerateUpdateDiffOperations(diffEntityConfiguration, existingEntity, newEntity, diffOperations);
                            diffEntityConfiguration.ValuesConfiguration.ValuesProperties.CopyPropertyValues(existingEntity, newEntity);
                        }

                        var diffModificationsFound = DiffUsingNavigation(diffEntityConfiguration, existingEntity, newEntity, diffOperations);
                        if (!areNewValuesEquals || diffModificationsFound)
                        {
                            if (!areNewValuesEquals || (diffModificationsFound && DiffSingleOrManyConfiguration.OnUpdateEvenIfModificationsDetectedOnlyInNestedLevel))
                                OnUpdate(diffEntityConfiguration, existingEntity, newEntity);
                            results.Add(existingEntity);
                        }
                    }
                }
                // existing entity not found in new entities -> it's a delete
                else
                {
                    OnDeleteAndPropagateUsingNavigation(diffEntityConfiguration, existingEntity); // once an entity is deleted, it's children will also be deleted
                    results.Add(existingEntity);
                }
            }

            // search if every new entity is found in existing entities -> this will detect insert
            foreach (var newEntity in newEntities)
            {
                Stopwatch.Restart();
                var newEntityFoundInExistingEntities = existingEntitiesHashtable != null
                    ? existingEntitiesHashtable.ContainsKey(newEntity)
                    : SearchMatchingEntityByKey(diffEntityConfiguration.KeyConfiguration, existingEntities, newEntity) != null;
                Stopwatch.Stop();
                Elapsed[ElapsedSearchEntity] += Stopwatch.ElapsedMilliseconds;
                // new entity not found in existing entity -> it's an insert
                if (!newEntityFoundInExistingEntities)
                {
                    OnInsertAndPropagateUsingNavigation(diffEntityConfiguration, newEntity); // once an entity is inserted, it's children will also be inserted
                    results.Add(newEntity);
                }
            }

            return results;
        }

        private bool CheckIfHashtablesShouldBeUsed(IEnumerable<object> existingEntities)
            => DiffSingleOrManyConfiguration.UseHashtable && existingEntities.Count() >= DiffSingleOrManyConfiguration.HashtableThreshold;

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

        private Hashtable InitializeHashtable(KeyConfiguration keyConfiguration, IEnumerable<object> entities)
        {
            Stopwatch.Restart();
            var equalityComparer = keyConfiguration.UsePrecompiledEqualityComparer
                ? keyConfiguration.PrecompiledEqualityComparer
                : keyConfiguration.NaiveEqualityComparer;
            var hashtable = new Hashtable(equalityComparer);
            foreach (var entity in entities)
                hashtable.Add(entity, entity);
            Stopwatch.Stop();
            Elapsed[ElapsedHashtableCreation] += Stopwatch.ElapsedMilliseconds;
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
            if (navigationManyConfiguration.NavigationManyProperty == null)
                return false;
            var childType = navigationManyConfiguration.NavigationManyChildType;
            if (childType == null)
                return false;

            if (!DiffEntityConfigurationByTypes.TryGetValue(childType, out var childDiffEntityConfiguration))
                throw new MissingConfigurationException(childType);

            var existingEntityChildren = navigationManyConfiguration.NavigationManyProperty.GetValue(existingEntity);
            var newEntityChildren = navigationManyConfiguration.NavigationManyProperty.GetValue(newEntity);

            // diff children
            var existingChildren = (IEnumerable<object>)existingEntityChildren!;
            var newChildren = (IEnumerable<object>)newEntityChildren!;
            var diffChildren = InternalDiffMany(childDiffEntityConfiguration, existingChildren, newChildren, diffOperations);

            // convert diff children from IEnumerable<object> to List<ChildType>
            Stopwatch.Restart();
            var listType = typeof(List<>).MakeGenericType(childType);
            var list = (IList)Activator.CreateInstance(listType)!;
            foreach (var diffChild in diffChildren)
                list.Add(diffChild);
            Stopwatch.Stop();
            Elapsed[ElapsedConvertToList] += Stopwatch.ElapsedMilliseconds;
            navigationManyConfiguration.NavigationManyProperty.SetValue(existingEntity, list);
            if (list.Count > 0)
                return true;
            return false;
        }

        private bool DiffUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations)
        {
            if (navigationOneConfiguration.NavigationOneProperty == null)
                return false;
            var childType = navigationOneConfiguration.NavigationOneChildType;
            if (childType == null)
                return false;

            if (!DiffEntityConfigurationByTypes.TryGetValue(childType, out var childDiffEntityConfiguration))
                throw new MissingConfigurationException(childType);

            var existingEntityChild = navigationOneConfiguration.NavigationOneProperty.GetValue(existingEntity);
            var newEntityChild = navigationOneConfiguration.NavigationOneProperty.GetValue(newEntity);

            // was not existing and is now new -> it's an insert
            if (existingEntityChild == null && newEntityChild != null)
            {
                navigationOneConfiguration.NavigationOneProperty.SetValue(existingEntity, newEntityChild);
                OnInsertAndPropagateUsingNavigation(childDiffEntityConfiguration, newEntityChild);
                return true;
            }
            // was existing and is not new -> it's a delete
            if (existingEntityChild != null && newEntityChild == null)
            {
                OnDeleteAndPropagateUsingNavigation(childDiffEntityConfiguration, existingEntityChild);
                return true;
            }
            // was existing and is new -> maybe an update
            if (existingEntityChild != null && newEntityChild != null)
            {
                bool areKeysEqual = false;
                if (childDiffEntityConfiguration.KeyConfiguration.KeyProperties != null)
                {
                    areKeysEqual = childDiffEntityConfiguration.KeyConfiguration.UsePrecompiledEqualityComparer
                        ? childDiffEntityConfiguration.KeyConfiguration.PrecompiledEqualityComparer.Equals(existingEntityChild, newEntityChild)
                        : childDiffEntityConfiguration.KeyConfiguration.NaiveEqualityComparer.Equals(existingEntityChild, newEntityChild);
                    if (!areKeysEqual) // keys are different -> copy keys
                        childDiffEntityConfiguration.KeyConfiguration.KeyProperties.CopyPropertyValues(existingEntityChild, newEntityChild);
                }

                bool areNewValuesEquals = false;
                if (childDiffEntityConfiguration.ValuesConfiguration?.ValuesProperties != null)
                {
                    areNewValuesEquals = childDiffEntityConfiguration.ValuesConfiguration.UsePrecompiledEqualityComparer
                        ? childDiffEntityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Equals(existingEntityChild, newEntityChild)
                        : childDiffEntityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Equals(existingEntityChild, newEntityChild);
                    // new values are different -> copy new values and dump differences
                    if (!areNewValuesEquals)
                    {
                        GenerateUpdateDiffOperations(childDiffEntityConfiguration, existingEntityChild, newEntityChild, diffOperations);
                        childDiffEntityConfiguration.ValuesConfiguration.ValuesProperties.CopyPropertyValues(existingEntityChild, newEntityChild);
                    }
                }

                var diffModificationsFound = DiffUsingNavigation(childDiffEntityConfiguration, existingEntityChild, newEntityChild, diffOperations);
                if (!areKeysEqual || !areNewValuesEquals || diffModificationsFound) // update
                {
                    if (!areKeysEqual || !areNewValuesEquals || (diffModificationsFound && DiffSingleOrManyConfiguration.OnUpdateEvenIfModificationsDetectedOnlyInNestedLevel))
                        OnUpdate(childDiffEntityConfiguration, existingEntityChild, newEntityChild);
                    return true;
                }
            }
            // not insert/update/delete -> set to null
            navigationOneConfiguration.NavigationOneProperty.SetValue(existingEntity, null);
            return false;
        }

        private void OnUpdate(DiffEntityConfiguration diffEntityConfiguration, object existingEntity, object newEntity)
        {
            if (diffEntityConfiguration.UpdateConfiguration == null)
                return;
            var updateConfiguration = diffEntityConfiguration.UpdateConfiguration;
            // SetValue
            updateConfiguration.SetValueConfiguration?.DestinationProperty.SetValue(existingEntity, updateConfiguration.SetValueConfiguration.Value);
            // CopyValues
            updateConfiguration.CopyValuesConfiguration?.CopyValuesProperties.CopyPropertyValues(existingEntity, newEntity);
        }

        private void OnInsertAndPropagateUsingNavigation(DiffEntityConfiguration diffEntityConfiguration, object entity)
        {
            if (diffEntityConfiguration.InsertConfiguration != null)
            {
                var insertConfiguration = diffEntityConfiguration.InsertConfiguration;
                // SetValue
                insertConfiguration.SetValueConfiguration?.DestinationProperty.SetValue(entity, insertConfiguration.SetValueConfiguration.Value);
            }
            PropagateUsingNavigation(diffEntityConfiguration, entity, OnInsertAndPropagateUsingNavigation);
        }

        private void OnDeleteAndPropagateUsingNavigation(DiffEntityConfiguration diffEntityConfiguration, object entity)
        {
            if (diffEntityConfiguration.DeleteConfiguration != null)
            {
                var deleteConfiguration = diffEntityConfiguration.DeleteConfiguration;
                // SetValue
                deleteConfiguration.SetValueConfiguration?.DestinationProperty.SetValue(entity, deleteConfiguration.SetValueConfiguration.Value);
            }
            PropagateUsingNavigation(diffEntityConfiguration, entity, OnDeleteAndPropagateUsingNavigation);
        }

        private void PropagateUsingNavigation(DiffEntityConfiguration diffEntityConfiguration, object entity, Action<DiffEntityConfiguration, object> operation)
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

        private void PropagateUsingNavigationMany(NavigationManyConfiguration navigationManyConfiguration, object entity, Action<DiffEntityConfiguration, object> operation)
        {
            if (navigationManyConfiguration.NavigationManyProperty == null)
                return;
            var childrenValue = navigationManyConfiguration.NavigationManyProperty.GetValue(entity);
            if (childrenValue == null)
                return;
            var childType = navigationManyConfiguration.NavigationManyChildType;
            if (childType == null)
                return;
            if (!DiffEntityConfigurationByTypes.TryGetValue(childType, out var childDiffEntityConfiguration))
                throw new MissingConfigurationException(childType);
            var children = (IEnumerable<object>)childrenValue;
            foreach (var child in children)
                operation(childDiffEntityConfiguration, child);
        }

        private void PropagateUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object entity, Action<DiffEntityConfiguration, object> operation)
        {
            if (navigationOneConfiguration.NavigationOneProperty == null)
                return;
            var childValue = navigationOneConfiguration.NavigationOneProperty.GetValue(entity);
            if (childValue == null)
                return;
            var childType = navigationOneConfiguration.NavigationOneChildType;
            if (childType == null)
                return;
            if (!DiffEntityConfigurationByTypes.TryGetValue(childType, out var childDiffEntityConfiguration))
                throw new MissingConfigurationException(childType);
            operation(childDiffEntityConfiguration, childValue);
        }

        private void GenerateUpdateDiffOperations(DiffEntityConfiguration diffEntityConfiguration, object existingEntity, object newEntity, IList<DiffOperationBase> diffOperations) 
        {
            if (diffEntityConfiguration.UpdateConfiguration?.GenerateOperations == true)
            {
                if (diffEntityConfiguration.ValuesConfiguration?.ValuesProperties != null)
                {
                    foreach (var propertyInfo in diffEntityConfiguration.ValuesConfiguration.ValuesProperties)
                    {
                        var existingValue = propertyInfo.GetValue(existingEntity);
                        var newValue = propertyInfo.GetValue(newEntity);
                        if (!Equals(existingValue, newValue))
                            diffOperations.Add(new UpdateDiffOperation
                            {
                                EntityName = existingEntity.GetType().Name,
                                PropertyName = propertyInfo.Name,
                                ExistingValue = existingValue?.ToString(),
                                NewValue = newValue?.ToString()
                            });
                    }
                }
            }
        }
    }
}
