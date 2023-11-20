using EntityMerger.Configuration;
using EntityMerger.UnitTest.Entities;
using EntityMerger.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace EntityMerger.UnitTest.Simple;

public class SimpleEntityNoNavigationTests
{
    [Fact]
    public void Identical()
    {
        var existing = Enumerable.Range(0, 5).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Existing{x}",
            AdditionalValueToCopy = $"CalculatedAdditionalValue{x}",
        }).ToArray();

        var calculated = Enumerable.Range(0, 5).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Calculated{x}",
            AdditionalValueToCopy = $"CalculatedAdditionalValue{x}",
        }).ToArray();

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.PersistEntity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasCalculatedValue(x => new { x.RequestedPower, x.Penalty });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Empty(results);
    }

    [Fact]
    public void OneDelete()
    {
        var existing = Enumerable.Range(0, 5).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Existing{x}",
            AdditionalValueToCopy = $"CalculatedAdditionalValue{x}",
        }).ToArray();

        // index 2 is missing -> will be marked as deleted
        var calculated = Enumerable.Range(0, 5).Except(new[] { 2 }).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Calculated{x}",
            AdditionalValueToCopy = $"CalculatedAdditionalValue{x}",
        }).ToArray();

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.PersistEntity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasCalculatedValue(x => new { x.RequestedPower, x.Penalty });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Delete, results.Single().PersistChange);
        Assert.Same(existing.Single(x => x.Index == 2), results.Single());
    }

    [Fact]
    public void OneInsert()
    {
        var existing = Enumerable.Range(0, 5).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Existing{x}",
            AdditionalValueToCopy = $"CalculatedAdditionalValue{x}",
        }).ToArray();

        // index 5 doesn't exist in existing collection -> will be marked as inserted
        var calculated = Enumerable.Range(0, 6).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Calculated{x}",
            AdditionalValueToCopy = $"CalculatedAdditionalValue{x}",
        }).ToArray();

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.PersistEntity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasCalculatedValue(x => new { x.RequestedPower, x.Penalty });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Insert, results.Single().PersistChange);
        Assert.Same(calculated.Single(x => x.Index == 5), results.Single());
    }

    [Fact]
    public void OneDelete_OneInsert()
    {
        var existing = Enumerable.Range(0, 5).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Existing{x}",
            AdditionalValueToCopy = $"CalculatedAdditionalValue{x}",
        }).ToArray();

        // index 2 is missing -> will be marked as deleted
        // index 5 doesn't exist in existing collection -> will be marked as inserted
        var calculated = Enumerable.Range(0, 6).Except(new[] { 2 }).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Calculated{x}",
            AdditionalValueToCopy = $"CalculatedAdditionalValue{x}",
        }).ToArray();

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.PersistEntity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasCalculatedValue(x => new { x.RequestedPower, x.Penalty });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Equal(2, results.Length);
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Insert));
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Delete));
        Assert.Same(calculated.Single(x => x.Index == 5), results.Single(x => x.PersistChange == PersistChange.Insert));
        Assert.Same(existing.Single(x => x.Index == 2), results.Single(x => x.PersistChange == PersistChange.Delete));
    }
}
