using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.Internal.Comparers;
using DeepDiff.Internal.Configuration;
using DeepDiff.Internal.Extensions;
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
        private IOperationListener OperationListener { get; }
        private Stack<(EntityConfiguration EntityConfiguration, KeyConfiguration KeyConfiguration, object Entity)> EntityNavigationPath { get; } = new Stack<(EntityConfiguration EntityConfiguration, KeyConfiguration KeyConfiguration, object Entity)>(10);

        private DeepDiffEngine(IReadOnlyDictionary<Type, EntityConfiguration> entityConfigurationByTypes, DiffEngineConfiguration diffEngineConfiguration, IOperationListener operationListener)
        {
            EntityConfigurationByTypes = entityConfigurationByTypes;
            DiffEngineConfiguration = diffEngineConfiguration;
            OperationListener = operationListener;
        }

        public static object MergeSingle(IReadOnlyDictionary<Type, EntityConfiguration> entityConfigurationByTypes, DiffEngineConfiguration diffEngineConfiguration, IOperationListener operationListener, Type entityType, EntityConfiguration entityConfiguration, object existingEntity, object newEntity)
        {
            var engine = new DeepDiffEngine(entityConfigurationByTypes, diffEngineConfiguration, operationListener);
            return engine.MergeSingleByType(entityType, entityConfiguration, existingEntity, newEntity);
        }

        public static List<object> MergeMany(IReadOnlyDictionary<Type, EntityConfiguration> entityConfigurationByTypes, DiffEngineConfiguration diffEngineConfiguration, IOperationListener operationListener, Type entityType, EntityConfiguration entityConfiguration, IEnumerable<object> existingEntities, IEnumerable<object> newEntities)
        {
            var engine = new DeepDiffEngine(entityConfigurationByTypes, diffEngineConfiguration, operationListener);
            return engine.MergeManyByType(entityType, entityConfiguration, existingEntities, newEntities);
        }

        private object MergeSingleByType(Type entityType, EntityConfiguration entityConfiguration, object existingEntity, object newEntity)
        {
            // no entity
            if (existingEntity == null && newEntity == null)
                return null;

            // no existing entity -> return new entity as inserted
            if (existingEntity == null)
            {
                OnInsertAndPropagateUsingNavigation(entityConfiguration, newEntity);
                return newEntity;
            }

            // if no new entity -> return existing as deleted
            if (newEntity == null)
            {
                OnDeleteAndPropagateUsingNavigation(entityConfiguration, existingEntity);
                return existingEntity;
            }

            // was existing and is new -> maybe an update
            bool areKeysEqual = true;
            if (!entityConfiguration.NoKey && entityConfiguration.KeyConfiguration.KeyProperties != null)
            {
                var keysComparer = entityConfiguration.KeyConfiguration.GetComparer(DiffEngineConfiguration.EqualityComparer);

                areKeysEqual = keysComparer.Equals(existingEntity, newEntity);
                if (!areKeysEqual && !DiffEngineConfiguration.CompareOnly) // keys are different -> copy keys
                    entityConfiguration.KeyConfiguration.KeyProperties.CopyPropertyValues(existingEntity, newEntity);
            }

            // compare values
            CompareByPropertyResult compareByPropertyResult = null;
            if (entityConfiguration.ValuesConfiguration?.ValuesProperties != null)
            {
                var valuesComparer = entityConfiguration.ValuesConfiguration.GetComparer(DiffEngineConfiguration.EqualityComparer);

                compareByPropertyResult = valuesComparer.Compare(existingEntity, newEntity);
            }

            // perform merge on nested entities
            var diffModificationsFound = MergeUsingNavigation(entityConfiguration, existingEntity, newEntity);

            // check force update if equals
            var forceOnUpdate = CheckIfOnUpdateHasToBeForced(entityConfiguration, existingEntity);

            //
            if (!areKeysEqual
                || (compareByPropertyResult != null && !compareByPropertyResult.IsEqual)
                || diffModificationsFound
                || forceOnUpdate) // update
            {
                if (!areKeysEqual
                    || (compareByPropertyResult != null && !compareByPropertyResult.IsEqual)
                    || (diffModificationsFound && (DiffEngineConfiguration.ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel || entityConfiguration.ForceUpdateIfConfiguration?.NestedEntitiesModifiedEnabled == true))
                    || forceOnUpdate)
                    OnUpdate(entityConfiguration, existingEntity, newEntity, compareByPropertyResult);
                return existingEntity;
            }

            return null;
        }

        private List<object> MergeManyByType(Type entityType, EntityConfiguration entityConfiguration, IEnumerable<object> existingEntities, IEnumerable<object> newEntities)
        {
            if (entityConfiguration.NoKey)
                throw new NoKeyEntityInNavigationManyException(entityType);

            var results = new List<object>();

            // no entities to merge
            if ((existingEntities == null || !existingEntities.Any()) && (newEntities == null || !newEntities.Any()))
                return results;

            // no existing entities -> return new as inserted
            if (existingEntities == null || !existingEntities.Any())
            {
                foreach (var newEntity in newEntities)
                {
                    OnInsertAndPropagateUsingNavigation(entityConfiguration, newEntity); // once an entity is inserted, it's children will also be inserted
                    results.Add(newEntity);
                }
                return results;
            }

            // no new entities -> return existing as deleted
            if (newEntities == null || !newEntities.Any())
            {
                foreach (var existingEntity in existingEntities)
                {
                    OnDeleteAndPropagateUsingNavigation(entityConfiguration, existingEntity); // once an entity is deleted, it's children will also be deleted
                    results.Add(existingEntity);
                }
                return results;
            }

            var keysComparer = entityConfiguration.KeyConfiguration.GetComparer(DiffEngineConfiguration.EqualityComparer);
            var valuesComparer = entityConfiguration.ValuesConfiguration?.GetComparer(DiffEngineConfiguration.EqualityComparer);

            // we are sure there is at least one existing and one new entity
            var existingEntitiesHashtable = CheckIfHashtablesShouldBeUsed(existingEntities)
                ? InitializeHashtable(keysComparer, existingEntities, entityConfiguration, entityConfiguration.KeyConfiguration)
                : null;
            var newEntitiesHashtable = CheckIfHashtablesShouldBeUsed(newEntities)
                ? InitializeHashtable(keysComparer, newEntities, entityConfiguration, entityConfiguration.KeyConfiguration)
                : null;

            // search if every existing entity is found in new entities -> this will detect update and delete
            foreach (var existingEntity in existingEntities)
            {
                var newEntity = newEntitiesHashtable != null
                    ? newEntitiesHashtable[existingEntity]
                    : SearchMatchingEntityByKey(keysComparer, newEntities, existingEntity, entityConfiguration, entityConfiguration.KeyConfiguration);
                // existing entity found in new entities -> maybe an update
                if (newEntity != null)
                {
                    // compare values
                    CompareByPropertyResult compareByPropertyResult = null;
                    if (valuesComparer != null)
                    {
                        compareByPropertyResult = valuesComparer.Compare(existingEntity, newEntity);
                    }

                    // perform merge on nested entities
                    var diffModificationsFound = MergeUsingNavigation(entityConfiguration, existingEntity, newEntity);

                    // check force update if equals
                    var forceOnUpdate = CheckIfOnUpdateHasToBeForced(entityConfiguration, existingEntity);

                    //
                    if ((compareByPropertyResult != null && !compareByPropertyResult.IsEqual)
                        || diffModificationsFound
                        || forceOnUpdate) // update
                    {
                        if ((compareByPropertyResult != null && !compareByPropertyResult.IsEqual)
                            || (diffModificationsFound && (DiffEngineConfiguration.ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel || entityConfiguration.ForceUpdateIfConfiguration?.NestedEntitiesModifiedEnabled == true))
                            || forceOnUpdate)
                            OnUpdate(entityConfiguration, existingEntity, newEntity, compareByPropertyResult);
                        results.Add(existingEntity);
                    }
                }
                // existing entity not found in new entities -> it's a delete
                else
                {
                    OnDeleteAndPropagateUsingNavigation(entityConfiguration, existingEntity); // once an entity is deleted, it's children will also be deleted
                    results.Add(existingEntity);
                }
            }

            // search if every new entity is found in existing entities -> this will detect insert
            foreach (var newEntity in newEntities)
            {
                var newEntityFoundInExistingEntities = existingEntitiesHashtable != null
                    ? existingEntitiesHashtable.ContainsKey(newEntity)
                    : SearchMatchingEntityByKey(keysComparer, existingEntities, newEntity, entityConfiguration, entityConfiguration.KeyConfiguration) != null;
                // new entity not found in existing entity -> it's an insert
                if (!newEntityFoundInExistingEntities)
                {
                    OnInsertAndPropagateUsingNavigation(entityConfiguration, newEntity); // once an entity is inserted, it's children will also be inserted
                    results.Add(newEntity);
                }
            }

            return results;
        }

        private List<object> MergeManyMultipleTypes(IEnumerable<object> existingEntities, IEnumerable<object> newEntities)
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
                var subResults = MergeManyByType(entityType, entityConfiguration, existingEntitiesByType, newEntitiesByType);
                results.AddRange(subResults);
            }

            return results;
        }

        private bool CheckIfHashtablesShouldBeUsed(IEnumerable<object> existingEntities)
            => DiffEngineConfiguration.UseHashtable && existingEntities.Count() >= DiffEngineConfiguration.HashtableThreshold;

        private static object SearchMatchingEntityByKey(IComparerByProperty keysComparer, IEnumerable<object> entities, object existingEntity, EntityConfiguration entityConfiguration, KeyConfiguration keyConfiguration)
        {
            try
            {
                return entities.SingleOrDefault(x => keysComparer.Equals(x, existingEntity));
            }
            catch (InvalidOperationException)
            {
                var keys = GenerateKeysForException(entityConfiguration, keyConfiguration, existingEntity);
                throw new DuplicateKeysException(entityConfiguration.EntityType, keys);
            }
        }

        private static Hashtable InitializeHashtable(IComparerByProperty keysComparer, IEnumerable<object> entities, EntityConfiguration entityConfiguration, KeyConfiguration keyConfiguration)
        {
            var hashtable = new Hashtable(keysComparer);
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

        private bool MergeUsingNavigation(EntityConfiguration entityConfiguration, object existingEntity, object newEntity)
        {
            var modificationsDetected = false;
            if (entityConfiguration.NavigationManyConfigurations != null)
            {
                foreach (var navigationManyConfiguration in entityConfiguration.NavigationManyConfigurations)
                {
                    EntityNavigationPath.Push((entityConfiguration, entityConfiguration.KeyConfiguration, existingEntity));
                    modificationsDetected |= MergeUsingNavigationMany(navigationManyConfiguration, existingEntity, newEntity);
                    EntityNavigationPath.Pop();
                }
            }
            if (entityConfiguration.NavigationOneConfigurations != null)
            {
                foreach (var navigationOneConfiguration in entityConfiguration.NavigationOneConfigurations)
                {
                    EntityNavigationPath.Push((entityConfiguration, entityConfiguration.KeyConfiguration, existingEntity));
                    modificationsDetected |= MergeUsingNavigationOne(navigationOneConfiguration, existingEntity, newEntity);
                    EntityNavigationPath.Pop();
                }
            }
            return modificationsDetected;
        }

        private bool MergeUsingNavigationMany(NavigationManyConfiguration navigationManyConfiguration, object existingEntity, object newEntity)
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
                mergedChildren = MergeManyMultipleTypes(existingChildren, newChildren);
            else
            {
                if (!EntityConfigurationByTypes.TryGetValue(childType, out var childEntityConfiguration))
                    throw new MissingConfigurationException(childType);
                mergedChildren = MergeManyByType(childType, childEntityConfiguration, existingChildren, newChildren);
            }

            // convert merged children from IEnumerable<object> to List<ChildType>
            var listType = typeof(List<>).MakeGenericType(childType);
            var list = (IList)Activator.CreateInstance(listType)!;
            foreach (var mergedChild in mergedChildren)
                list.Add(mergedChild);
            // set navigation many property to merged children
            if (!DiffEngineConfiguration.CompareOnly)
                navigationManyConfiguration.NavigationProperty.SetValue(existingEntity, list);
            //
            if (list.Count > 0)
                return true;
            return false;
        }

        private bool MergeUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object existingEntity, object newEntity)
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
            var mergedChild = MergeSingleByType(childType, childEntityConfiguration, existingEntityChild, newEntityChild);

            // set navigation one property to merged child
            if (!DiffEngineConfiguration.CompareOnly)
                navigationOneConfiguration.NavigationProperty.SetValue(existingEntity, mergedChild);

            // not insert/delete/update
            if (mergedChild == null)
                return false;

            return true;
        }

        private void OnUpdate(EntityConfiguration entityConfiguration, object existingEntity, object newEntity, CompareByPropertyResult compareByPropertyResult)
        {
            if (entityConfiguration.UpdateConfiguration != null)
            {
                var updateConfiguration = entityConfiguration.UpdateConfiguration;
                // use CopyValues from UpdateConfiguration
                OnUpdateCopyValues(updateConfiguration, existingEntity, newEntity);
                // use SetValue from UpdateConfiguration
                OnUpdateSetValue(updateConfiguration, existingEntity);
            }
            // copy values from ValuesConfiguration, only updated ones
            OnUpdateCopyModifiedValues(entityConfiguration, existingEntity, compareByPropertyResult);
        }

        private void OnUpdateCopyModifiedValues(EntityConfiguration entityConfiguration, object existingEntity, CompareByPropertyResult compareByPropertyResult)
        {
            if (compareByPropertyResult != null && !compareByPropertyResult.IsEqual)
            {
                foreach (var compareByPropertyResultDetail in compareByPropertyResult.Details.Where(x => entityConfiguration.ValuesConfiguration.ValuesProperties.Contains(x.PropertyInfo))) // copy modified properties found in values configuration (modified properties will always be a subset of values but we are testing to be sure)
                {
                    // notify update
                    OperationListener?.OnUpdate(entityConfiguration.EntityType.Name, compareByPropertyResultDetail.PropertyInfo.Name, () => GenerateKeysForOperation(entityConfiguration, entityConfiguration.KeyConfiguration, existingEntity), () => compareByPropertyResultDetail.OldValue, () => compareByPropertyResultDetail.NewValue, () => GenerateNavigationPathKeysForOperation());
                    //
                    if (!DiffEngineConfiguration.CompareOnly)
                        compareByPropertyResultDetail.PropertyInfo.SetValue(existingEntity, compareByPropertyResultDetail.NewValue);
                }
            }
        }

        private void OnUpdateSetValue(UpdateConfiguration updateConfiguration, object existingEntity)
        {
            if (updateConfiguration.SetValueConfigurations != null && updateConfiguration.SetValueConfigurations.Count > 0 && !DiffEngineConfiguration.CompareOnly)
            {
                foreach (var setValueConfiguration in updateConfiguration.SetValueConfigurations)
                    setValueConfiguration?.DestinationProperty.SetValue(existingEntity, setValueConfiguration.Value);
            }
        }

        private void OnUpdateCopyValues(UpdateConfiguration updateConfiguration, object existingEntity, object newEntity)
        {
            //
            if (!DiffEngineConfiguration.CompareOnly)
                updateConfiguration.CopyValuesConfiguration?.CopyValuesProperties.CopyPropertyValues(existingEntity, newEntity);
        }

        private void OnInsertAndPropagateUsingNavigation(EntityConfiguration entityConfiguration, object newEntity)
        {
            // notify insert
            OperationListener?.OnInsert(entityConfiguration.EntityType.Name, () => GenerateKeysForOperation(entityConfiguration, entityConfiguration.KeyConfiguration, newEntity), () => GenerateNavigationPathKeysForOperation());
            //
            if (entityConfiguration.InsertConfiguration != null)
            {
                var insertConfiguration = entityConfiguration.InsertConfiguration;
                // use SetValue from InsertConfiguration
                if (insertConfiguration.SetValueConfigurations != null && insertConfiguration.SetValueConfigurations.Count > 0 && !DiffEngineConfiguration.CompareOnly)
                {
                    foreach(var setValueConfiguration in insertConfiguration.SetValueConfigurations)
                        setValueConfiguration.DestinationProperty.SetValue(newEntity, setValueConfiguration.Value);
                }
            }

            PropagateUsingNavigation(entityConfiguration, newEntity, (childEntityConfiguration, parentNavigationConfiguration, child, parent) => OnInsertAndPropagateUsingNavigation(childEntityConfiguration, child));
        }

        private void OnDeleteAndPropagateUsingNavigation(EntityConfiguration entityConfiguration, object existingEntity)
        {
            // notify delete
            OperationListener?.OnDelete(entityConfiguration.EntityType.Name, () => GenerateKeysForOperation(entityConfiguration, entityConfiguration.KeyConfiguration, existingEntity), () => GenerateNavigationPathKeysForOperation());
            //
            if (entityConfiguration.DeleteConfiguration != null)
            {
                var deleteConfiguration = entityConfiguration.DeleteConfiguration;
                // use SetValue from DeleteConfiguration
                if (deleteConfiguration.SetValueConfigurations != null && deleteConfiguration.SetValueConfigurations.Count > 0 && !DiffEngineConfiguration.CompareOnly)
                {
                    foreach (var setValueConfiguration in deleteConfiguration.SetValueConfigurations)
                        setValueConfiguration.DestinationProperty.SetValue(existingEntity, setValueConfiguration.Value);
                }
            }
            PropagateUsingNavigation(entityConfiguration, existingEntity, (childEntityConfiguration, parentNavigationConfiguration, child, parent) => OnDeleteAndPropagateUsingNavigation(childEntityConfiguration, child));
        }

        private void PropagateUsingNavigation(EntityConfiguration entityConfiguration, object entity, Action<EntityConfiguration, NavigationConfigurationBase, object, object> operation)
        {
            if (entityConfiguration.NavigationManyConfigurations != null)
            {
                foreach (var navigationManyConfiguration in entityConfiguration.NavigationManyConfigurations)
                {
                    EntityNavigationPath.Push((entityConfiguration, entityConfiguration.KeyConfiguration, entity));
                    PropagateUsingNavigationManyMultipleTypes(navigationManyConfiguration, entity, operation);
                    EntityNavigationPath.Pop();
                }
            }
            if (entityConfiguration.NavigationOneConfigurations != null)
            {
                foreach (var navigationOneConfiguration in entityConfiguration.NavigationOneConfigurations)
                {
                    EntityNavigationPath.Push((entityConfiguration, entityConfiguration.KeyConfiguration, entity));
                    PropagateUsingNavigationOne(navigationOneConfiguration, entity, operation);
                    EntityNavigationPath.Pop();
                }
            }
        }

        private void PropagateUsingNavigationManyMultipleTypes(NavigationManyConfiguration navigationManyConfiguration, object entity, Action<EntityConfiguration, NavigationConfigurationBase, object, object> operation)
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
                    PropagateUsingNavigationManyByType(navigationManyConfiguration, entity, childType, childrenByType, operation);
                }
            }
            else
            {
                var childType = navigationManyConfiguration.NavigationChildType;
                PropagateUsingNavigationManyByType(navigationManyConfiguration, entity, childType, children, operation);
            }
        }

        private void PropagateUsingNavigationManyByType(NavigationManyConfiguration navigationManyConfiguration, object entity, Type childType, IEnumerable<object> children, Action<EntityConfiguration, NavigationConfigurationBase, object, object> operation)
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

        private static Dictionary<string, object> GenerateKeysForOperation(EntityConfiguration entityConfiguration, KeyConfiguration keyConfiguration, object entity)
        {
            if (entityConfiguration.NoKey)
                return null;

            var result = new Dictionary<string, object>();
            foreach (var propertyInfo in keyConfiguration.KeyProperties)
            {
                var key = propertyInfo.GetValue(entity);
                result.Add(propertyInfo.Name, key);
            }
            return result;
        }

        private Dictionary<string, Dictionary<string, object>> GenerateNavigationPathKeysForOperation()
        {
            var result = new Dictionary<string, Dictionary<string, object>>();
            foreach (var entry in EntityNavigationPath)
            {
                var keys = GenerateKeysForOperation(entry.EntityConfiguration, entry.KeyConfiguration, entry.Entity);
                result.Add(entry.EntityConfiguration.EntityType.Name, keys);
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
