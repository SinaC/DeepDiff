using EntityMerger.EntityMerger;
using System.Collections;
using System.Reflection;

namespace EntityMerger;

public class Merger
{
    private MergeConfiguration Configuration { get; }

    internal Merger(MergeConfiguration configuration)
    {
        Configuration = configuration;
    }

    // for each existing
    //      if found in calculated (based on keys)
    //          if values not the same
    //              copy values
    //          merge existing.Many  TODO
    //          merge existing.One   TODO
    //          if values not the same or something modified when merging Many and One
    //              mark existing as Updated
    //      else
    //          mark existing as Deleted
    //          mark existing.Many as Deleted
    //          mark existing.One as Deleted
    //  for each calculated
    //      if not found in existing
    //          mark calculated as Inserted
    //          mark calculated.Many as Inserted
    //          mark calculated.One as Inserted
    public IEnumerable<TEntity> Merge<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> calculatedEntities)
        where TEntity : PersistEntity
    {
        if (!Configuration.MergeEntityConfigurations.TryGetValue(typeof(TEntity), out var mergeEntityConfiguration))
            yield break; // TODO: exception

        var mergedEntities = Merge(mergeEntityConfiguration, existingEntities, calculatedEntities);
        foreach (var mergedEntity in mergedEntities)
        {
            yield return (TEntity)mergedEntity;
        }
    }

    private IEnumerable<PersistEntity> Merge(MergeEntityConfiguration mergeEntityConfiguration, IEnumerable<PersistEntity> existingEntities, IEnumerable<PersistEntity> calculatedEntities)
    {
        // search if every existing entity is found in calculated entities -> this will detect update and delete
        foreach (var existingEntity in existingEntities)
        {
            var existingEntityFoundInCalculatedEntities = false;
            foreach (var calculatedEntity in calculatedEntities)
            {
                var areKeysEqual = AreEqualByPropertyInfos(mergeEntityConfiguration.KeyProperties, existingEntity, calculatedEntity);

                // existing entity found in calculated entities -> if values are different it's an update
                if (areKeysEqual)
                {
                    var areValuesEquals = AreEqualByPropertyInfos(mergeEntityConfiguration.ValueProperties, existingEntity, calculatedEntity);
                    if (!areValuesEquals) // values are different -> it's an update
                    {
                        CopyValuesFromCalculatedToExistingByPropertyInfos(mergeEntityConfiguration.ValueProperties, existingEntity, calculatedEntity);
                        // TODO: move out of if and don't yield return yet, detect if merge navigation has modified something and yield return if !areValuesEquals or something modified in navigation
                        MergeUsingNavigation(mergeEntityConfiguration, existingEntity, calculatedEntity);
                        existingEntity.PersistChange = PersistChange.Update;
                        yield return existingEntity;
                    }

                    existingEntityFoundInCalculatedEntities = true;
                    break; // don't need to check other calculated entities
                }
            }
            // existing entity not found in calculated entities -> it's a delete
            if (!existingEntityFoundInCalculatedEntities)
            {
                existingEntity.PersistChange = PersistChange.Delete;
                PropagatePersistChangeUsingNavigation(mergeEntityConfiguration, existingEntity, PersistChange.Delete); // once an entity is deleted, it's children will also be deleted
                yield return existingEntity;
            }
        }

        // search if every calculated entity is found in existing entities -> this will detect insert
        foreach (var calculatedEntity in calculatedEntities)
        {
            var calculatedEntityFoundInExistingEntities = false;
            foreach (var existingEntity in existingEntities)
            {
                var areKeysEqual = AreEqualByPropertyInfos(mergeEntityConfiguration.KeyProperties, existingEntity, calculatedEntity);
                if (areKeysEqual)
                {
                    calculatedEntityFoundInExistingEntities = true;
                    break; // don't need to check other existing entities
                }
            }
            // calculated entity not found in existing entity -> it's an insert
            if (!calculatedEntityFoundInExistingEntities)
            {
                calculatedEntity.PersistChange = PersistChange.Insert;
                PropagatePersistChangeUsingNavigation(mergeEntityConfiguration, calculatedEntity, PersistChange.Insert); // once an entity is inserted, it's children will also be inserted
                yield return calculatedEntity;
            }
        }
    }

    private void MergeUsingNavigation(MergeEntityConfiguration mergeEntityConfiguration, PersistEntity existingEntity, PersistEntity calculatedEntity)
    {
        if (mergeEntityConfiguration.NavigationManyProperties != null)
        {
            foreach (var navigationManyProperty in mergeEntityConfiguration.NavigationManyProperties)
                MergeUsingNavigationMany(navigationManyProperty, existingEntity, calculatedEntity);
        }
        if (mergeEntityConfiguration.NavigationOneProperties != null)
        {
            foreach (var navigationOneProperty in mergeEntityConfiguration.NavigationOneProperties)
                MergeUsingNavigationOne(navigationOneProperty, existingEntity, calculatedEntity);
        }
    }

    private void MergeUsingNavigationMany(PropertyInfo navigationProperty, PersistEntity existingEntity, PersistEntity calculatedEntity)
    {
        if (navigationProperty == null)
            return;
        var childType = GetChildType(navigationProperty);
        if (childType == null)
            return;

        if (!Configuration.MergeEntityConfigurations.TryGetValue(childType, out var childMergeEntityConfiguration))
            return; // TODO: exception

        var existingEntityChildren = navigationProperty.GetValue(existingEntity);
        var calculatedEntityChildren = navigationProperty.GetValue(calculatedEntity);

        // merge children
        var mergedChildren = Merge(childMergeEntityConfiguration, (IEnumerable<PersistEntity>)existingEntityChildren, (IEnumerable<PersistEntity>)calculatedEntityChildren);

        // convert children from List<PersistEntity> to List<EnityType>
        var listType = typeof(List<>).MakeGenericType(childType);
        var list = (IList)Activator.CreateInstance(listType);
        foreach (var mergedChild in mergedChildren)
            list.Add(mergedChild);
        if (list.Count > 0)
            navigationProperty.SetValue(existingEntity, list);
    }

    private void MergeUsingNavigationOne(PropertyInfo navigationProperty, PersistEntity existingEntity, PersistEntity calculatedEntity)
    {
        if (navigationProperty == null)
            return;
        var childType = GetChildType(navigationProperty);
        if (childType == null)
            return;

        if (!Configuration.MergeEntityConfigurations.TryGetValue(childType, out var childMergeEntityConfiguration))
            return; // TODO: exception

        var existingEntityChild = navigationProperty.GetValue(existingEntity);
        var calculatedEntityChild = navigationProperty.GetValue(calculatedEntity);

        // was not existing and is now calculated -> it's an insert
        if (existingEntityChild == null && calculatedEntityChild != null)
        {

            navigationProperty.SetValue(existingEntity, calculatedEntityChild);
        }
        // TODO
    }

    private void CopyValuesFromCalculatedToExistingByPropertyInfos(IEnumerable<PropertyInfo> propertyInfos, object existingEntity, object calculatedEntity)
    {
        foreach (var propertyInfo in propertyInfos)
        {
            var calculatedValue = propertyInfo.GetValue(calculatedEntity);
            propertyInfo.SetValue(existingEntity, calculatedValue);
        }
    }

    private void PropagatePersistChangeUsingNavigation(MergeEntityConfiguration mergeEntityConfiguration, object entity, PersistChange persistChange)
    {
        if (mergeEntityConfiguration.NavigationManyProperties != null)
        {
            foreach (var navigationManyProperty in mergeEntityConfiguration.NavigationManyProperties)
                PropagatePersistChangeUsingNavigationMany(navigationManyProperty, entity, persistChange);
        }
        if (mergeEntityConfiguration.NavigationOneProperties != null)
        {
            foreach (var navigationOneProperty in mergeEntityConfiguration.NavigationOneProperties)
                PropagatePersistChangeUsingNavigationOne(navigationOneProperty, entity, persistChange);
        }
    }

    private void PropagatePersistChangeUsingNavigationMany(PropertyInfo navigationProperty, object entity, PersistChange persistChange)
    {
        if (navigationProperty == null)
            return;
        var childrenValue = navigationProperty.GetValue(entity);
        if (childrenValue == null)
            return;
        var children = (IEnumerable<PersistEntity>)childrenValue;
        foreach (var child in children)
        {
            child.PersistChange = persistChange;
        }
    }

    private void PropagatePersistChangeUsingNavigationOne(PropertyInfo navigationProperty, object entity, PersistChange persistChange)
    {
        if (navigationProperty == null)
            return;
        var childValue = navigationProperty.GetValue(entity);
        if (childValue == null)
            return;
        var child = (PersistEntity)childValue;
        child.PersistChange = persistChange;
    }

    private bool AreEqualByPropertyInfos(IEnumerable<PropertyInfo> propertyInfos, object existingEntity, object calculatedEntity)
    {
        foreach (var propertyInfo in propertyInfos)
        {
            var existingValue = propertyInfo.GetValue(existingEntity);
            var calculatedValue = propertyInfo.GetValue(calculatedEntity);

            if (!Equals(existingValue, calculatedValue))
                return false;
        }
        return true;
    }

    private Type GetChildType(PropertyInfo childrenProperty)
    {
        Type type = childrenProperty.PropertyType;
        // check List<>
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            return type.GetGenericArguments()[0];
        }
        // check IList<>
        var interfaceTest = new Func<Type, Type>(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>) ? i.GetGenericArguments().Single() : null);
        var innerType = interfaceTest(type);
        if (innerType != null)
            return innerType;
        foreach (var i in type.GetInterfaces())
        {
            innerType = interfaceTest(i);
            if (innerType != null)
                return innerType;
        }
        //
        return null;
    }
}
