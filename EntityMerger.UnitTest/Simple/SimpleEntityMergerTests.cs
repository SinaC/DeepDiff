using EntityMerger.EntityMerger;
using EntityMerger.UnitTest.Entities;
using EntityMerger.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace EntityMerger.UnitTest.Simple;

public partial class SimpleEntityMergerTests
{
    [Fact]
    public void Test1()
    {
        var existing = Enumerable.Range(0, 10).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Existing{x}",
            SubEntities = Enumerable.Range(0, 5).Select(y => new SubEntity
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddHours(x).AddMinutes(y),
                Power = y,
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"Existing{x}_{y}",
            }).ToList(),
        }).ToArray();

        var calculated = Enumerable.Range(1, 10).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 3,
            Comment = $"Calculated{x}",
            SubEntities = Enumerable.Range(1, 5).Select(y => new SubEntity
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddHours(x).AddMinutes(y),
                Power = y,
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"Calculated{x}_{y}",
            }).ToList(),
        }).ToArray();

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.PersistEntity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasCalculatedValue(x => new { x.RequestedPower, x.Penalty })
            .HasMany(x => x.SubEntities);
        mergeConfiguration.PersistEntity<SubEntity>()
            .HasKey(x => x.Timestamp)
            .HasCalculatedValue(x => new { x.Power, x.Price });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Equal(11, results.Length);
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Insert));
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Delete));
        Assert.Equal(9, results.Count(x => x.PersistChange == PersistChange.Update));
    }
}
