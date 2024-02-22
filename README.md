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
Then in your application code
```csharp
var result = deepDiff.DiffMany(existingEntities, newEntities); // result.Entities will contain 'diff' entities
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
