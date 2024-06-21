# Sample

## How do I get started
First configure DeepDiff to know what types you want to compare, in the startup of your application

```csharp
var diffConfiguration = new DiffConfiguration();
diffConfiguration.Entity<Entity>()
   .HasKey(x => new { x.StartsOn, x.Name })
   .HasValues(x => new { x.Price, x.Volume })
   .HasMany(x => x.SubEntities)
   .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
   .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
   .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));
diffConfiguration.Entity<SubEntity>()
   .HasKey(x => x.SubName)
   .HasValues(x => x.Energy)
   .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
   .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
   .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
var deepDiff = diffConfiguration.CreateDeepDiff();
```
Then in your application code, this will detect insert/update/delete between existing and new entities. In case of update, properties will be copied from new to existing entity
```csharp
var result = deepDiff.MergeMany(existingEntities, newEntities); // result.Entities will contain 'diff' entities
```
Sample entities definition
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
  public PersistChange PersistChange { get; set; }
  public Guid EntityId { get; set; } // Foreign Key
}

public enum PersistChange
{
  None,
  Inserted,
  Updated,
  Deleted
}
```

# Entity Configuration

## HasKey

Defines properties used to compare and detect an insert and a delete. Mandatory unless NoKey has been defined

```csharp
IEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression)
IEntityConfiguration<TEntity> HasKey<TKey>(Expression<Func<TEntity, TKey>> keyExpression, Action<IKeyConfiguration<TEntity>> keyConfigurationAction)
```

### UsePrecompiledEqualityComparer

When set to true, engine will use optimized equality comparers to compare keys and values (true by default)

```csharp
IKeyConfiguration<TEntity> UsePrecompiledEqualityComparer(bool use = true)
```

## HasValues

Defines properties used to detect an update in case keys are identical

```csharp
IEntityConfiguration<TEntity> HasValues<TValue>(Expression<Func<TEntity, TValue>> valuesExpression)
IEntityConfiguration<TEntity> HasValues<TValue>(Expression<Func<TEntity, TValue>> valuesExpression, Action<IValuesConfiguration<TEntity>> valuesConfigurationAction)
```

### UsePrecompiledEqualityComparer

When set to true, engine will use optimized equality comparers to compare keys and values (true by default)

```csharp
IValuesConfiguration<TEntity> UsePrecompiledEqualityComparer(bool use = true)
```

## HasOne

Defines property to navigate to a single child

```csharp
IEntityConfiguration<TEntity> HasOne<TChildEntity>(Expression<Func<TEntity, TChildEntity>> navigationPropertyExpression)
IEntityConfiguration<TEntity> HasOne<TChildEntity>(Expression<Func<TEntity, TChildEntity>> navigationPropertyExpression, Action<INavigationOneConfiguration<TEntity, TChildEntity>> navigationOneConfigurationAction)
```

## HasMany

Defines property to navigation to multiple children

```csharp
IEntityConfiguration<TEntity> HasMany<TChildEntity>(Expression<Func<TEntity, List<TChildEntity>>> navigationPropertyExpression)
IEntityConfiguration<TEntity> HasMany<TChildEntity>(Expression<Func<TEntity, List<TChildEntity>>> navigationPropertyExpression, Action<INavigationManyConfiguration<TEntity, TChildEntity>> navigationManyConfigurationAction)
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

### GenerateOperations

When set to true, insert operations will be logged (true by default) in the result

```csharp
IInsertConfiguration<TEntity> GenerateOperations(bool generate = true)
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

### GenerateOperations

When set to true, update operations will be logged (true by default) in the result

```csharp
IUpdateConfiguration<TEntity> GenerateOperations(bool generate = true)
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

### GenerateOperations

When set to true, delete operations will be logged (true by default) in the result

```csharp
IDeleteConfiguration<TEntity> GenerateOperations(bool generate = true)
```

## WithComparer

Defines the IEqualityComparer to use when comparing entity of that type

```csharp
IEntityConfiguration<TEntity> WithComparer<T>(IEqualityComparer<T> equalityComparer)
```

## ForceUpdateIf

Defines additional criteria to detect an update even if Values are identical

```csharp
IEntityConfiguration<TEntity> ForceUpdateIf(Action<IForceUpdateIfConfiguration<TEntity>> forceUpdateIfConfigurationAction)
```

### NestedEntitiesModified

Trigger an update when a nested entity is modified

```csharp
IForceUpdateIfConfiguration<TEntity> NestedEntitiesModified()
```

### Equals

Trigger an update when an equality condition is set

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
MergeSingleResult<TEntity> MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity)
MergeSingleResult<TEntity> MergeSingle<TEntity>(TEntity existingEntity, TEntity newEntity, Action<IMergeSingleConfiguration> mergeSingleConfigurationAction)
```

## MergeMany

```csharp
MergeManyResult<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities)
MergeManyResult<TEntity> MergeMany<TEntity>(IEnumerable<TEntity> existingEntities, IEnumerable<TEntity> newEntities, Action<IMergeManyConfiguration> mergeManyConfigurationAction)
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

### ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel

Force OnUpdate to be triggered if a nested entity has been modified even if current entity is not modified

```csharp
IMergeSingleConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(bool force = false)
```

### GenerateOperations

When set to true, engine will generate a collection of operations detected when performing diff (true by default)

```csharp
IMergeSingleConfiguration GenerateOperations(bool generate = true)
```

### UsePrecompiledEqualityComparer

When set to true, engine will use optimized equality comparers to compare keys and values (true by default)

```csharp
IMergeSingleConfiguration UsePrecompiledEqualityComparer(bool use = true)
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

### ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel

Force OnUpdate to be triggered if a nested entity has been modified even if current entity is not modified

```csharp
IMergeManyConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(bool force = false)
```

### GenerateOperations

When set to true, engine will generate a collection of operations detected when performing diff (true by default)

```csharp
IMergeManyConfiguration GenerateOperations(bool generate = true)
```

### UsePrecompiledEqualityComparer

When set to true, engine will use optimized equality comparers to compare keys and values (true by default)

```csharp
IMergeManyConfiguration UsePrecompiledEqualityComparer(bool use = true)
```

# Deep Diff Configuration

## Entity

Create an entity configuration

```csharp
IEntityConfiguration<TEntity> Entity<TEntity>()
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

# Diff operations

## InsertDiffOperation

### Keys

Keys of entity on which insert has been performed

### EntityName

Name of entity on which insert has been performed

## UpdateDiffOperation

### Keys

Keys of entity on which update has been performed

### EntityName

Name of entity on which update has been performed

### UpdatedProperties

Collection of properties from Values configuration on which update has been performed

### SetValueProperties

Collection of properties on which SetValue has been performed

### CopyValuesProperties

Collection of properties on which CopyValues has been performed

## DeleteDiffOperation

### Keys

Keys of entity on which delete has been performed

### EntityName

Name of entity on which delete has been performed
