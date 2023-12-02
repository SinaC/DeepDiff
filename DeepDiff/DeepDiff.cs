using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DeepDiff
{
    internal sealed class DeepDiff : IDeepDiff
    {
        private DiffConfiguration Configuration { get; }

        internal DeepDiff(DiffConfiguration configuration)
        {
            Configuration = configuration;
        }

        public TEntity DiffSingle<TEntity>(TEntity existingEntity, TEntity newEntity)
            where TEntity : class
        {
            if (!Configuration.DiffEntityConfigurationByTypes.TryGetValue(typeof(TEntity), out var diffEntityConfiguration))
                throw new MissingConfigurationException(typeof(TEntity));
            return (TEntity)DiffSingle(diffEntityConfiguration, existingEntity, newEntity);
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
        public IEnumerable<TEntity> DiffMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
            where TEntity : class
        {
            if (!Configuration.DiffEntityConfigurationByTypes.TryGetValue(typeof(TEntity), out var diffEntityConfiguration))
                throw new MissingConfigurationException(typeof(TEntity));

            var diffEntities = DiffMany(diffEntityConfiguration, existingEntities, newEntities, Configuration.UseHashtable);
            foreach (var diffEntity in diffEntities)
                yield return (TEntity)diffEntity;
        }

        private bool CheckIfHashtablesShouldBeUsedUsingThreshold(IEnumerable<object> existingEntities)
            => existingEntities.Count() >= Configuration.HashtableThreshold;

        private object DiffSingle(DiffEntityConfiguration diffEntityConfiguration, object existingEntity, object newEntity)
        {
            // no entity
            if (existingEntity == null && newEntity == null)
                return null;

            // no existing entity -> return new entity as inserted
            if (existingEntity == null)
            {
                MarkEntityAndPropagateUsingNavigation(diffEntityConfiguration, newEntity, DiffEntityOperation.Insert);
                return newEntity;
            }

            // if no new entity -> return existing as deleted
            if (newEntity == null)
            {
                MarkEntityAndPropagateUsingNavigation(diffEntityConfiguration, existingEntity, DiffEntityOperation.Delete);
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
            if (diffEntityConfiguration.ValuesConfiguration.ValuesProperties != null)
            {
                areNewValuesEquals = diffEntityConfiguration.ValuesConfiguration.UsePrecompiledEqualityComparer
                    ? diffEntityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, newEntity)
                    : diffEntityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Equals(existingEntity, newEntity);
                // new values are different -> copy new values and additional values to copy
                if (!areNewValuesEquals)
                {
                    diffEntityConfiguration.ValuesConfiguration.ValuesProperties.CopyPropertyValues(existingEntity, newEntity);
                    diffEntityConfiguration.AdditionalValuesToCopyConfiguration?.AdditionalValuesToCopyProperties.CopyPropertyValues(existingEntity, newEntity);
                }
            }

            var diffModificationsFound = DiffUsingNavigation(diffEntityConfiguration, existingEntity, newEntity);

            if (!areKeysEqual || !areNewValuesEquals || diffModificationsFound) // update
            {
                MarkEntity(diffEntityConfiguration, existingEntity, DiffEntityOperation.Update);
                return existingEntity;
            }

            return null; // not insert/update/delete
        }

        private IEnumerable<object> DiffMany(DiffEntityConfiguration diffEntityConfiguration, IEnumerable<object> existingEntities, IEnumerable<object> newEntities, bool useHashtableForThatLevel)
        {
            // no entities to diff
            if ((existingEntities == null || !existingEntities.Any()) && (newEntities == null || !newEntities.Any()))
                yield break;

            // no existing entities -> return new as inserted
            if (existingEntities == null || !existingEntities.Any())
            {
                foreach (var newEntity in newEntities)
                {
                    MarkEntityAndPropagateUsingNavigation(diffEntityConfiguration, newEntity, DiffEntityOperation.Insert); // once an entity is inserted, it's children will also be inserted
                    yield return newEntity;
                }
                yield break;
            }

            // no new entities -> return existing as deleted
            if (newEntities == null || !newEntities.Any())
            {
                foreach (var existingEntity in existingEntities)
                {
                    MarkEntityAndPropagateUsingNavigation(diffEntityConfiguration, existingEntity, DiffEntityOperation.Delete); // once an entity is deleted, it's children will also be deleted
                    yield return existingEntity;
                }
                yield break;
            }

            // we are sure there is at least one existing and one new entity
            var existingEntitiesHashtable = useHashtableForThatLevel && Configuration.UseHashtable && CheckIfHashtablesShouldBeUsedUsingThreshold(existingEntities)
                ? InitializeHashtable(diffEntityConfiguration.KeyConfiguration, existingEntities)
                : null;
            var newEntitiesHashtable = useHashtableForThatLevel && Configuration.UseHashtable && CheckIfHashtablesShouldBeUsedUsingThreshold(newEntities)
                ? InitializeHashtable(diffEntityConfiguration.KeyConfiguration, newEntities)
                : null;

            // search if every existing entity is found in new entities -> this will detect update and delete
            foreach (var existingEntity in existingEntities)
            {
                var newEntity = Configuration.UseHashtable && newEntitiesHashtable != null
                    ? newEntitiesHashtable[existingEntity]
                    : SearchMatchingEntityByKey(diffEntityConfiguration.KeyConfiguration, newEntities, existingEntity);
                // existing entity found in new entities -> if values are different it's an update
                if (newEntity != null)
                {
                    if (diffEntityConfiguration.ValuesConfiguration.ValuesProperties != null)
                    {
                        var areNewValuesEquals = diffEntityConfiguration.ValuesConfiguration.UsePrecompiledEqualityComparer
                            ? diffEntityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Equals(existingEntity, newEntity)
                            : diffEntityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Equals(existingEntity, newEntity);
                        // new values are different -> copy new values and additional values to copy
                        if (!areNewValuesEquals)
                        {
                            diffEntityConfiguration.ValuesConfiguration.ValuesProperties.CopyPropertyValues(existingEntity, newEntity);
                            diffEntityConfiguration.AdditionalValuesToCopyConfiguration?.AdditionalValuesToCopyProperties.CopyPropertyValues(existingEntity, newEntity);
                        }

                        var diffModificationsFound = DiffUsingNavigation(diffEntityConfiguration, existingEntity, newEntity);
                        if (!areNewValuesEquals || diffModificationsFound)
                        {
                            MarkEntity(diffEntityConfiguration, existingEntity, DiffEntityOperation.Update);
                            yield return existingEntity;
                        }
                    }
                }
                // existing entity not found in new entities -> it's a delete
                else
                {
                    MarkEntityAndPropagateUsingNavigation(diffEntityConfiguration, existingEntity, DiffEntityOperation.Delete); // once an entity is deleted, it's children will also be deleted
                    yield return existingEntity;
                }
            }

            // search if every new entity is found in existing entities -> this will detect insert
            foreach (var newEntity in newEntities)
            {
                var newEntityFoundInExistingEntities = Configuration.UseHashtable && existingEntitiesHashtable != null
                    ? existingEntitiesHashtable.ContainsKey(newEntity)
                    : SearchMatchingEntityByKey(diffEntityConfiguration.KeyConfiguration, existingEntities, newEntity) != null;
                // new entity not found in existing entity -> it's an insert
                if (!newEntityFoundInExistingEntities)
                {
                    MarkEntityAndPropagateUsingNavigation(diffEntityConfiguration, newEntity, DiffEntityOperation.Insert); // once an entity is inserted, it's children will also be inserted
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

        private bool DiffUsingNavigation(DiffEntityConfiguration diffEntityConfiguration, object existingEntity, object newEntity)
        {
            var modificationsDetected = false;
            if (diffEntityConfiguration.NavigationManyConfigurations != null)
            {
                foreach (var navigationManyConfiguration in diffEntityConfiguration.NavigationManyConfigurations)
                    modificationsDetected |= DiffUsingNavigationMany(navigationManyConfiguration, existingEntity, newEntity);
            }
            if (diffEntityConfiguration.NavigationOneConfigurations != null)
            {
                foreach (var navigationOneConfiguration in diffEntityConfiguration.NavigationOneConfigurations)
                    modificationsDetected |= DiffUsingNavigationOne(navigationOneConfiguration, existingEntity, newEntity);
            }
            return modificationsDetected;
        }

        private bool DiffUsingNavigationMany(NavigationManyConfiguration navigationManyConfiguration, object existingEntity, object newEntity)
        {
            if (navigationManyConfiguration.NavigationManyProperty == null)
                return false;
            var childType = navigationManyConfiguration.NavigationManyChildType;
            if (childType == null)
                return false;

            if (!Configuration.DiffEntityConfigurationByTypes.TryGetValue(childType, out var childDiffEntityConfiguration))
                throw new MissingConfigurationException(childType);

            var existingEntityChildren = navigationManyConfiguration.NavigationManyProperty.GetValue(existingEntity);
            var newEntityChildren = navigationManyConfiguration.NavigationManyProperty.GetValue(newEntity);

            // diff children
            var existingChildren = (IEnumerable<object>)existingEntityChildren!;
            var newChildren = (IEnumerable<object>)newEntityChildren!;
            var diffChildren = DiffMany(childDiffEntityConfiguration, existingChildren, newChildren, navigationManyConfiguration.UseHashtable);

            // convert diff children from IEnumerable<object> to List<ChildType>
            var listType = typeof(List<>).MakeGenericType(childType);
            var list = (IList)Activator.CreateInstance(listType)!;
            foreach (var diffChild in diffChildren)
                list.Add(diffChild);
            navigationManyConfiguration.NavigationManyProperty.SetValue(existingEntity, list);
            if (list.Count > 0)
                return true;
            return false;
        }

        private bool DiffUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object existingEntity, object newEntity)
        {
            if (navigationOneConfiguration.NavigationOneProperty == null)
                return false;
            var childType = navigationOneConfiguration.NavigationOneChildType;
            if (childType == null)
                return false;

            if (!Configuration.DiffEntityConfigurationByTypes.TryGetValue(childType, out var childDiffEntityConfiguration))
                throw new MissingConfigurationException(childType);

            var existingEntityChild = navigationOneConfiguration.NavigationOneProperty.GetValue(existingEntity);
            var newEntityChild = navigationOneConfiguration.NavigationOneProperty.GetValue(newEntity);

            // was not existing and is now new -> it's an insert
            if (existingEntityChild == null && newEntityChild != null)
            {
                navigationOneConfiguration.NavigationOneProperty.SetValue(existingEntity, newEntityChild);
                MarkEntityAndPropagateUsingNavigation(childDiffEntityConfiguration, newEntityChild, DiffEntityOperation.Insert);
                return true;
            }
            // was existing and is not new -> it's a delete
            if (existingEntityChild != null && newEntityChild == null)
            {
                MarkEntityAndPropagateUsingNavigation(childDiffEntityConfiguration, existingEntityChild, DiffEntityOperation.Delete);
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
                if (childDiffEntityConfiguration.ValuesConfiguration.ValuesProperties != null)
                {
                    areNewValuesEquals = childDiffEntityConfiguration.ValuesConfiguration.UsePrecompiledEqualityComparer
                        ? childDiffEntityConfiguration.ValuesConfiguration.PrecompiledEqualityComparer.Equals(existingEntityChild, newEntityChild)
                        : childDiffEntityConfiguration.ValuesConfiguration.NaiveEqualityComparer.Equals(existingEntityChild, newEntityChild);
                    // new values are different -> copy new values and additional values to copy
                    if (!areNewValuesEquals)
                    {
                        childDiffEntityConfiguration.ValuesConfiguration.ValuesProperties.CopyPropertyValues(existingEntityChild, newEntityChild);
                        childDiffEntityConfiguration.AdditionalValuesToCopyConfiguration?.AdditionalValuesToCopyProperties.CopyPropertyValues(existingEntityChild, newEntityChild);
                    }
                }

                var diffModificationsFound = DiffUsingNavigation(childDiffEntityConfiguration, existingEntityChild, newEntityChild);

                if (!areKeysEqual || !areNewValuesEquals || diffModificationsFound) // update
                {
                    MarkEntity(childDiffEntityConfiguration, existingEntityChild, DiffEntityOperation.Update);
                    return true;
                }
            }
            // not insert/update/delete -> set to null
            navigationOneConfiguration.NavigationOneProperty.SetValue(existingEntity, null);
            return false;
        }

        private static void MarkEntity(DiffEntityConfiguration diffEntityConfiguration, object entity, DiffEntityOperation operation)
        {
            if (!diffEntityConfiguration.MarkAsByOperation.TryGetValue(operation, out var markAsConfiguration))
                throw new MissingMarkAsConfigurationException(entity.GetType(), operation);
            markAsConfiguration.DestinationProperty.SetValue(entity, markAsConfiguration.Value);
        }

        private void MarkEntityAndPropagateUsingNavigation(DiffEntityConfiguration diffEntityConfiguration, object entity, DiffEntityOperation operation)
        {
            MarkEntity(diffEntityConfiguration, entity, operation);
            PropagateUsingNavigation(diffEntityConfiguration, entity, operation);
        }

        private void PropagateUsingNavigation(DiffEntityConfiguration diffEntityConfiguration, object entity, DiffEntityOperation operation)
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

        private void PropagateUsingNavigationMany(NavigationManyConfiguration navigationManyConfiguration, object entity, DiffEntityOperation operation)
        {
            if (navigationManyConfiguration.NavigationManyProperty == null)
                return;
            var childrenValue = navigationManyConfiguration.NavigationManyProperty.GetValue(entity);
            if (childrenValue == null)
                return;
            var childType = navigationManyConfiguration.NavigationManyChildType;
            if (childType == null)
                return;
            if (!Configuration.DiffEntityConfigurationByTypes.TryGetValue(childType, out var childDiffEntityConfiguration))
                throw new MissingConfigurationException(childType);
            if (!childDiffEntityConfiguration.MarkAsByOperation.TryGetValue(operation, out var markAsConfiguration))
                throw new MissingMarkAsConfigurationException(entity.GetType(), operation);
            var children = (IEnumerable<object>)childrenValue;
            foreach (var child in children)
                MarkEntityAndPropagateUsingNavigation(childDiffEntityConfiguration, child, operation);
        }

        private void PropagateUsingNavigationOne(NavigationOneConfiguration navigationOneConfiguration, object entity, DiffEntityOperation operation)
        {
            if (navigationOneConfiguration.NavigationOneProperty == null)
                return;
            var childValue = navigationOneConfiguration.NavigationOneProperty.GetValue(entity);
            if (childValue == null)
                return;
            var childType = navigationOneConfiguration.NavigationOneChildType;
            if (childType == null)
                return;
            if (!Configuration.DiffEntityConfigurationByTypes.TryGetValue(childType, out var childDiffEntityConfiguration))
                throw new MissingConfigurationException(childType);
            MarkEntityAndPropagateUsingNavigation(childDiffEntityConfiguration, childValue, operation);
        }
    }
}