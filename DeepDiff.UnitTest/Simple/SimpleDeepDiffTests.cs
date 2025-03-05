using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Simple;

public partial class SimpleDeepDiffTests
{
    [Theory]
    [InlineData(EqualityComparers.Precompiled)]
    [InlineData(EqualityComparers.Naive)]
    public void TestMultipleChanges_MultipleEntities(EqualityComparers equalityComparer)
    {
        var existingEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel0
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Existing{x}",
            AdditionalValueToCopy = $"ExistingAdditionalValue{x}",
            SubEntities = Enumerable.Range(0, 5).Select(y => new EntityLevel1
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddHours(x).AddMinutes(y),
                Power = y,
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"Existing{x}_{y}",
            }).ToList(),
        }).ToArray();

        var newEntities = Enumerable.Range(1, 10).Select(x => new EntityLevel0
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 3,
            Comment = $"New{x}",
            AdditionalValueToCopy = $"NewAdditionalValue{x}",
            SubEntities = Enumerable.Range(1, 5).Select(y => new EntityLevel1
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddHours(x).AddMinutes(y),
                Power = y,
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"New{x}_{y}",
            }).ToList(),
        }).ToArray();

        DeepDiffConfiguration diffConfiguration = new DeepDiffConfiguration();
        diffConfiguration.Entity<EntityLevel0>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .OnUpdate(cfg => cfg.CopyValues(x => x.AdditionalValueToCopy))
            .HasMany(x => x.SubEntities);
        diffConfiguration.Entity<EntityLevel1>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power, x.Price });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var results = deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.SetEqualityComparer(equalityComparer)).ToArray();

        // deleted: 0
        // updated: 1, 2, 4, 5, 7, 8
        // none: 3, 6, 9 because keys and values are identical, only sub entities has been modified
        // inserted: 10
        Assert.Equal(11, results.Length);
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Insert));
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Delete));
        Assert.Equal(6, results.Count(x => x.PersistChange == PersistChange.Update)); 
        Assert.Equal(3, results.Count(x => x.PersistChange == PersistChange.None));
        Assert.All(results.Where(x => x.PersistChange == PersistChange.Insert), x => Assert.Equal(5, x.SubEntities.Count()));
        Assert.All(results.Where(x => x.PersistChange == PersistChange.Insert), x => Assert.All(x.SubEntities, y => Assert.Equal(PersistChange.Insert, y.PersistChange)));
        Assert.All(results.Where(x => x.PersistChange == PersistChange.Delete), x => Assert.Equal(5, x.SubEntities.Count()));
        Assert.All(results.Where(x => x.PersistChange == PersistChange.Delete), x => Assert.All(x.SubEntities, y => Assert.Equal(PersistChange.Delete, y.PersistChange)));
        Assert.All(results.Where(x => x.PersistChange == PersistChange.Update), x => Assert.Equal(2, x.SubEntities.Count()));
        Assert.All(results.Where(x => x.PersistChange == PersistChange.Update), x => Assert.Single(x.SubEntities.Where(y => y.PersistChange == PersistChange.Insert)));
        Assert.All(results.Where(x => x.PersistChange == PersistChange.Update), x => Assert.Single(x.SubEntities.Where(y => y.PersistChange == PersistChange.Delete)));
        Assert.All(results.Where(x => x.PersistChange == PersistChange.None), x => Assert.Equal(2, x.SubEntities.Count()));
        Assert.All(results.Where(x => x.PersistChange == PersistChange.None), x => Assert.Single(x.SubEntities.Where(y => y.PersistChange == PersistChange.Insert)));
        Assert.All(results.Where(x => x.PersistChange == PersistChange.None), x => Assert.Single(x.SubEntities.Where(y => y.PersistChange == PersistChange.Delete)));

        Assert.All(results.Where(x => x.PersistChange != PersistChange.Insert), x => Assert.StartsWith("Existing", x.Comment)); // Comment is not copied
        Assert.StartsWith("NewAdditionalValue", results.Single(x => x.PersistChange == PersistChange.Insert).AdditionalValueToCopy); // AdditionalValueToCopy is copied
    }
}
