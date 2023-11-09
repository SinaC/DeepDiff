using EntityMerger.EntityMerger;
using TestApp;

var existing = Enumerable.Range(0, 10).Select(x => new Entity
{
    Id = Guid.NewGuid(),
    StartsOn = DateTime.Today.AddHours(x),
    Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
    RequestedPower = x,
    Penalty = x % 3 == 0 ? (decimal?)null : x * 2,
    Comment = $"Existing{x}",
    SubEntities = Enumerable.Range(0, 5).Select(y => new SubEntity
    {
        Id = Guid.NewGuid(),
        Timestamp = DateTime.Today.AddHours(x).AddMinutes(y),
        Power = y,
        Price = y % 2 == 0 ? (decimal?)null : y * 3,
        Comment = $"Existing{x}_{y}",
    }).ToList()
}).ToArray();

var calculated = Enumerable.Range(1, 10).Select(x => new Entity
{
    Id = Guid.NewGuid(),
    StartsOn = DateTime.Today.AddHours(x),
    Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
    RequestedPower = x,
    Penalty = x % 3 == 0 ? (decimal?)null : x * 3,
    Comment = $"Calculated{x}",
    SubEntities = Enumerable.Range(1, 5).Select(y => new SubEntity
    {
        Id = Guid.NewGuid(),
        Timestamp = DateTime.Today.AddHours(x).AddMinutes(y),
        Power = y,
        Price = y % 2 == 0 ? (decimal?)null : y * 3,
        Comment = $"Calculated{x}_{y}",
    }).ToList()
}).ToArray();

MergeConfiguration mergeConfiguration = new MergeConfiguration();
mergeConfiguration.Entity<Entity>()
    .HasKey(x => new { x.StartsOn, x.Direction })
    .HasValue(x => new { x.RequestedPower, x.Penalty })
    .HasMany(x => x.SubEntities);
mergeConfiguration.Entity<SubEntity>()
    .HasKey(x => x.Timestamp)
    .HasValue(x => new { x.Power, x.Price });

var results = mergeConfiguration.Merge(existing, calculated).ToArray();

Console.WriteLine("Woot");