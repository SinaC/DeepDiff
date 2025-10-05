# Breaking changes

### 1.11.1

- By default, DeepDiff will now check for duplicate keys in entities and nested entities. You can disable this behavior using SetCheckDuplicateKeys(false) in MergeSingle, MergeMany, CompareSingle and CompareMany configuration

### 1.11.0

- IOperationListener are now mandatory when using CompareSingle and CompareMany methods

### 1.10.0

- to configure an entity, you must now use the `ConfigureEntity<TEntity>()` method instead of `Entity<TEntity>()`

# Example

## How do I get started
First configure DeepDiff to register what types you want to compare, in the startup of your application

```csharp
var diffConfiguration = new DiffConfiguration();
diffConfiguration.ConfigureEntity<Entity>()                       // configure Entity type
   .HasKey(x => new { x.StartsOn, x.Name })                       // business key
   .HasValues(x => new { x.Price, x.Volume })                     // properties to compare for update (and copy on update)
   .HasMany(x => x.SubEntities)                                   // one-to-many relation
   .OnInsert(cfg => cfg                                           // operations to perform on insert
          .SetValue(x => x.PersistChange, PersistChange.Insert))  //     change PersistChange property to Insert
   .OnUpdate(cfg => cfg                                           // operations to perform on update
          .SetValue(x => x.PersistChange, PersistChange.Update))  //     change PersistChange property to Update
   .OnDelete(cfg => cfg                                           // operations to perform on delete
          .SetValue(x => x.PersistChange, PersistChange.Delete)); //     change PersistChange property to Delete

diffConfiguration.ConfigureEntity<SubEntity>()                    // configure SubEntity type
   .HasKey(x => x.SubName)                                        // business key
   .HasValues(x => x.Energy)                                      // property to compare for update (and copy on update)
   .OnInsert(cfg => cfg                                           // operations to perform on insert
          .SetValue(x => x.PersistChange, PersistChange.Insert))  //     change PersistChange property to Insert
   .OnUpdate(cfg => cfg                                           // operations to perform on update
          .SetValue(x => x.PersistChange, PersistChange.Update)   //     change PersistChange property to Update
          .CopyValues(x => x.Description)                         //     copy Description property from new to existing entity
   .OnDelete(cfg => cfg                                           // operations to perform on delete
          .SetValue(x => x.PersistChange, PersistChange.Delete))  //     change PersistChange property to Delete
   .Ignore(x => new { x.Entity, x.EntityId} );                    //     foreign key and navigation are not relevant for diff

var deepDiff = diffConfiguration.CreateDeepDiff();                // finalize configuration
```

Then in your application code, this will detect insert/update/delete between existing and new entities. In case of update, properties will be copied from new to existing entity.
PersistChange property will be set accordingly, so you can persist changes in your database and in case of update Description property will also be copied from new to existing sub entity.

```csharp
var resultEntities = deepDiff.MergeMany(existingEntities, newEntities); // resultEntities will contain 'merged' entities
```

Example entities definition
```csharp
public class Entity
{
  public Guid Id { get; set; } // DB Key
  public DateTime StartsOn { get; set; } // Business Key
  public string Name { get; set; } // Business Key
  public decimal Price { get; set; }
  public int Volume { get; set; }
  public PersistChange PersistChange { get; set; }
  public List<SubEntity> SubEntities { get; set; }
}

public class SubEntity
{
  public Guid Id { get; set; } // DB Key
  public string SubName { get; set; } // Business Key
  public int Energy { get; set; }
  public string Description { get; set; } // Not part of key or values, will not be used in diff
  public PersistChange PersistChange { get; set; }
  public Guid EntityId { get; set; } // Foreign Key
  public Entity Entity { get; set; } // Navigation property
}

public enum PersistChange
{
  None,
  Insert,
  Update,
  Delete
}
```

# Entity Configuration

## HasKey

Defines properties used to compare and detect an insert and a delete. Mandatory unless NoKey has been defined

```csharp
IEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression)
IEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression, Action<IKeyConfiguration<TEntity>> keyConfigurationAction)
```

## HasValues

Defines properties used to detect an update in case keys are identical

```csharp
IEntityConfiguration<TEntity> HasValues<TValue>(Expression<Func<TEntity, TValue>> valuesExpression)
IEntityConfiguration<TEntity> HasValues<TValue>(Expression<Func<TEntity, TValue>> valuesExpression, Action<IValuesConfiguration<TEntity>> valuesConfigurationAction)
```

## HasOne

Defines property to navigate to a single child

```csharp
IEntityConfiguration<TEntity> HasOne<TChildEntity>(Expression<Func<TEntity, TChildEntity>> navigationPropertyExpression)
IEntityConfiguration<TEntity> HasOne<TChildEntity>(Expression<Func<TEntity, TChildEntity>> navigationPropertyExpression, Action<INavigationOneConfiguration<TEntity, TChildEntity>> navigationOneConfigurationAction)
```

## HasMany

Defines property to navigate to multiple children

```csharp
IEntityConfiguration<TEntity> HasMany<TChildEntity>(Expression<Func<TEntity, List<TChildEntity>>> navigationPropertyExpression)
IEntityConfiguration<TEntity> HasMany<TChildEntity>(Expression<Func<TEntity, List<TChildEntity>>> navigationPropertyExpression, Action<INavigationManyConfiguration<TEntity, TChildEntity>> navigationManyConfigurationAction)
```

### UseDerivedTypes

When set to true, engine will use force comparison by type if children are inherited and children collection is not abstract

```csharp
INavigationManyConfiguration<TEntity, TChildEntity> UseDerivedTypes(bool use = false)
```

## OnInsert

Defines operations to perform when an insert is detected

```csharp
IEntityConfiguration<TEntity> OnInsert(Action<IInsertConfiguration<TEntity>> insertConfigurationAction)
```

### SetValue

When an insert is detected, overwrite a property with a specific value

```csharp
IInsertConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value)
```

## OnUpdate

Defines operations to perform when an insert is detected. Properties specified in Values(...) will automatically be copied from new entity to existing one

```csharp
IEntityConfiguration<TEntity> OnUpdate(Action<IUpdateConfiguration<TEntity>> updateConfigurationAction)
```

### SetValue

When an update is detected, overwrite a property with a specific value

```csharp
IUpdateConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value)
```

### CopyValues

When an update is detected, specify additional properties to copy from new entity to existing one

```csharp
IUpdateConfiguration<TEntity> CopyValues<TValue>(Expression<Func<TEntity, TValue>> copyValuesExpression)
```

## OnDelete

Defines operations to perform when a delete is detected.

```csharp
IEntityConfiguration<TEntity> OnDelete(Action<IDeleteConfiguration<TEntity>> deleteConfigurationAction)
```

### SetValue

When a delete is detected, overwrite property with a specific value

```csharp
IDeleteConfiguration<TEntity> SetValue<TMember>(Expression<Func<TEntity, TMember>> destinationMember, TMember value)
```

## WithComparer

Defines the IEqualityComparer to use when comparing entity of that type

```csharp
IEntityConfiguration<TEntity> WithComparer<T>(IEqualityComparer<T> equalityComparer)
```

## ForceUpdateIf

Defines additional criteria to force an update even if no update is detected using entity value(s).

```csharp
IEntityConfiguration<TEntity> ForceUpdateIf(Action<IForceUpdateIfConfiguration<TEntity>> forceUpdateIfConfigurationAction)
```

### NestedEntitiesModified

Trigger an update when a nested entity is modified

```csharp
IForceUpdateIfConfiguration<TEntity> NestedEntitiesModified()
```

### Equals

Trigger an update when an equality condition is met

```csharp
IForceUpdateIfConfiguration<TEntity> Equals<TMember>(Expression<Func<TEntity, TMember>> compareToMember, TMember compareToValue)
```

## Ignore

Defines entity properties which will not be found anywhere in the entity diff configuration. This will be used by ValidateIfEveryPropertiesAreReferenced

```csharp
IEntityConfiguration<TEntity> Ignore<TIgnore>(Expression<Func<TEntity, TIgnore>> ignoreExpression)
```

## NoKey

Defines a no key entity, only update will be deteted for this kind of entity. Mandatory if no HasKey has been defined

```csharp
IEntityConfiguration<TEntity> NoKey()
```

# Engine configuration

## MergeSingle

```csharp
TEntity MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity)
TEntity MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener operationListener)
TEntity MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity, Action<IMergeSingleConfiguration> mergeSingleConfigurationAction)
TEntity MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener operationListener, Action<IMergeSingleConfiguration> mergeSingleConfigurationAction)
```

## MergeMany

```csharp
IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener operationListener)
IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, Action<IMergeManyConfiguration> mergeManyConfigurationAction)
IEnumerable<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener operationListener, Action<IMergeManyConfiguration> mergeManyConfigurationAction)
```

## CompareSingle

```csharp
void CompareSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener operationListener);
void CompareSingle<TEntity>(TEntity existingEntity, TEntity newEntity, IOperationListener operationListener, Action<ICompareSingleConfiguration> compareSingleConfigurationAction);
```

## CompareMany

```csharp
void CompareMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener operationListener);
void CompareMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, IOperationListener operationListener, Action<ICompareManyConfiguration> compareManyConfigurationAction);
```

## MergeSingle configuration

### UseHashtable

When set to true, hashtable will be used when searching in a collection of entities with a minimum HashtableThreshold (15 by default) entries (true by default)

```csharp
IMergeSingleConfiguration UseHashtable(bool use = true)
```

### HashtableThreshold

Defines minimum number of entries in collection to use hashtable (15 by default)

```csharp
IMergeSingleConfiguration HashtableThreshold(int threshold = 15)
```

### ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel

Force OnUpdate to be triggered if a nested entity has been modified even if current entity is not modified (false by default))

```csharp
IMergeSingleConfiguration ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(bool force = false)
```

### SetEqualityComparer

Choose which equality comparer engine will use to compare keys and values (Precompiled by default)

```csharp
IMergeSingleConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled)
```

### SetCheckDuplicateKeys

Indicates whether to check for duplicate keys in entities or nested entities (true by default).

```csharp
IMergeSingleConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true)
```

## MergeMany configuration

### UseHashtable

When set to true, hashtable will be used when searching in a collection of entities with a minimum HashtableThreshold (15 by default) entries (true by default)

```csharp
IMergeManyConfiguration UseHashtable(bool use = true)
```

### HashtableThreshold

Defines minimum number of entries in collection to use hashtable (15 by default)

```csharp
IMergeManyConfiguration HashtableThreshold(int threshold = 15)
```

### ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel

Force OnUpdate to be triggered if a nested entity has been modified even if current entity is not modified

```csharp
IMergeManyConfiguration ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(bool force = false)
```

### SetEqualityComparer

Choose which equality comparer engine will use to compare keys and values (Precompiled by default)

```csharp
IMergeManyConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled)
```

### SetCheckDuplicateKeys

Indicates whether to check for duplicate keys in entities or nested entities (true by default).

```csharp
IMergeManyConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true)
```

## CompareSingle configuration

### UseHashtable

When set to true, hashtable will be used when searching in a collection of entities with a minimum HashtableThreshold (15 by default) entries (true by default)

```csharp
ICompareSingleConfiguration UseHashtable(bool use = true);
```

### HashtableThreshold

Defines minimum number of entries in collection to use hashtable (15 by default)

```csharp
ICompareSingleConfiguration HashtableThreshold(int threshold = 15);
```

### SetEqualityComparer

Choose which equality comparer engine will use to compare keys and values (Precompiled by default)

```csharp
ICompareSingleConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled)
```

### SetCheckDuplicateKeys

Indicates whether to check for duplicate keys in entities or nested entities (true by default).

```csharp
ICompareSingleConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true)
```

## CompareMany configuration

### UseHashtable

When set to true, hashtable will be used when searching in a collection of entities with a minimum HashtableThreshold (15 by default) entries (true by default)

```csharp
ICompareManyConfiguration UseHashtable(bool use = true);
```

### HashtableThreshold

Defines minimum number of entries in collection to use hashtable (15 by default)

```csharp
ICompareManyConfiguration HashtableThreshold(int threshold = 15);
```

### SetEqualityComparer

Choose which equality comparer engine will use to compare keys and values (Precompiled by default)

```csharp
ICompareManyConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled)
```

### SetCheckDuplicateKeys

Indicates whether to check for duplicate keys in entities or nested entities (true by default).

```csharp
ICompareManyConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true)
```

## OperationListener

Defines a listener which will be called on every insert/update/delete detection

### OnInsert

```csharp
void OnDelete(string entityTypeName, Func<Dictionary<string, object>> getKeysFunc, Func<Dictionary<string, Dictionary<string, object>>> getNavigationParentKeysFunc);
```

### OnDelete

```csharp
void OnInsert(string entityTypeName, Func<Dictionary<string, object>> getKeysFunc, Func<Dictionary<string, Dictionary<string, object>>> getNavigationParentKeysFunc);
```

### OnUpdate

```csharp
void OnUpdate(string entityTypeName, string propertyName, Func<Dictionary<string, object>> getKeysFunc, Func<object> getOriginalValueFunc, Func<Dictionary<string, Dictionary<string, object>>> getNavigationParentKeysFunc);
```

# Deep Diff Configuration

## ConfigureEntity

Create an entity configuration

```csharp
IEntityConfiguration<TEntity> ConfigureEntity<TEntity>()
```

## AddProfile

Add diff profile

```csharp
IDeepDiffConfiguration AddProfile<TProfile>()
IDeepDiffConfiguration AddProfile(DiffProfile diffProfile)
```

## AddProfiles

Scan assemblies and add diff profile found in those assemblies

```csharp
IDeepDiffConfiguration AddProfiles(params Assembly[] assembliesToScan)
```

## CreateDeepDiff

Create DeepDiff engine (also validate configuration)

```csharp
IDeepDiff CreateDeepDiff()
```

## ValidateConfiguration

Validate entity configurations

```csharp
void ValidateConfiguration()
```

## ValidateIfEveryPropertiesAreReferenced

Check if every properties found in configured entities are used in configuration, use Ignore to add properties to ignore in this validation

```csharp
void ValidateIfEveryPropertiesAreReferenced()
```
