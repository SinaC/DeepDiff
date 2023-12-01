# Sample

## How do I get started
First configure DeepDiff to know what types you want to compare, in the startup of your application

```csharp
var diffConfiguration = new DiffConfiguration();
diffConfiguration.Entity<Entity>()
   .HasKey(x => new { x.StartsOn, x.Name })
   .HasValues(x => new { x.Price, x.Volume })
   .HasMany(x => x.SubEntities)
   .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
   .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
   .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);;
diffConfiguration.PersistEntity<SubEntity>()
   .HasKey(x => x.SubName)
   .HasValues(x => x.Energy)
   .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
   .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
   .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);;
var deepDiff = diffConfiguration.CreateDeepDiff();
```
Then in your application code
```csharp
var results = deepDiff.Diff(existingEntities, newEntities).ToArray();
```
Sample entities definition
```csharp
public class Entity
{
  public Guid Id { get; set; } // DB Key
  public DateTime StartsOn { get; set; } // Business Key
  public string Name { get; set; } // Business Key
  public int Price { get; set; }
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
}

public enum PersistChange
{
  None,
  Inserted,
  Updated,
  Deleted
}
```
