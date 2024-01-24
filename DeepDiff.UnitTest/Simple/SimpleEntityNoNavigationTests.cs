using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Simple;

public class SimpleEntityNoNavigationTests
{
    [Fact]
    public void Identical()
    {
        var existingEntities = Enumerable.Range(0, 5).Select(x => new EntityLevel0
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Existing{x}",
            AdditionalValueToCopy = $"ExistingAdditionalValue{x}",
        }).ToArray();

        var newEntities = Enumerable.Range(0, 5).Select(x => new EntityLevel0
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"New{x}",
            AdditionalValueToCopy = $"NewAdditionalValue{x}",
        }).ToArray();

        DiffConfiguration diffConfiguration = new DiffConfiguration();
        diffConfiguration.PersistEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var diff = deepDiff.DiffMany(existingEntities, newEntities);
        var results = diff.Entities.ToArray();

        Assert.Empty(results);
    }

    [Fact]
    public void OneDelete()
    {
        var existingEntities = Enumerable.Range(0, 5).Select(x => new EntityLevel0
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Existing{x}",
            AdditionalValueToCopy = $"NewAdditionalValue{x}",
        }).ToArray();

        // index 2 is missing -> will be marked as deleted
        var newEntities = Enumerable.Range(0, 5).Except(new[] { 2 }).Select(x => new EntityLevel0
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"New{x}",
            AdditionalValueToCopy = $"NewAdditionalValue{x}",
        }).ToArray();

        DiffConfiguration diffConfiguration = new DiffConfiguration();
        diffConfiguration.PersistEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var diff = deepDiff.DiffMany(existingEntities, newEntities);
        var results = diff.Entities.ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Delete, results.Single().PersistChange);
        Assert.Same(existingEntities.Single(x => x.Index == 2), results.Single());
    }

    [Fact]
    public void OneInsert()
    {
        var existingEntities = Enumerable.Range(0, 5).Select(x => new EntityLevel0
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Existing{x}",
            AdditionalValueToCopy = $"ExistingAdditionalValue{x}",
        }).ToArray();

        // index 5 doesn't exist in existing collection -> will be marked as inserted
        var newEntities = Enumerable.Range(0, 6).Select(x => new EntityLevel0
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"New{x}",
            AdditionalValueToCopy = $"NewAdditionalValue{x}",
        }).ToArray();

        DiffConfiguration diffConfiguration = new DiffConfiguration();
        diffConfiguration.PersistEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var diff = deepDiff.DiffMany(existingEntities, newEntities);
        var results = diff.Entities.ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Insert, results.Single().PersistChange);
        Assert.Same(newEntities.Single(x => x.Index == 5), results.Single());
    }

    [Fact]
    public void OneDelete_OneInsert()
    {
        var existingEntities = Enumerable.Range(0, 5).Select(x => new EntityLevel0
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"Existing{x}",
            AdditionalValueToCopy = $"ExistingAdditionalValue{x}",
        }).ToArray();

        // index 2 is missing -> will be marked as deleted
        // index 5 doesn't exist in existing collection -> will be marked as inserted
        var newEntities = Enumerable.Range(0, 6).Except(new[] { 2 }).Select(x => new EntityLevel0
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? null : x * 2,
            Comment = $"New{x}",
            AdditionalValueToCopy = $"NewAdditionalValue{x}",
        }).ToArray();

        DiffConfiguration diffConfiguration = new DiffConfiguration();
        diffConfiguration.PersistEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var diff = deepDiff.DiffMany(existingEntities, newEntities);
        var results = diff.Entities.ToArray();

        Assert.Equal(2, results.Length);
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Insert));
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Delete));
        Assert.Same(newEntities.Single(x => x.Index == 5), results.Single(x => x.PersistChange == PersistChange.Insert));
        Assert.Same(existingEntities.Single(x => x.Index == 2), results.Single(x => x.PersistChange == PersistChange.Delete));
    }
}
