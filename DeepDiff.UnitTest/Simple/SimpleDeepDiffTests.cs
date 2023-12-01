using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Simple;

public partial class SimpleDeepDiffTests
{
    [Fact]
    public void TestMultipleChanges_SingleEntity()
    {
        var existingEntity = new EntityLevel0
        {
            Index = 1,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today,
            Direction = Direction.Up,
            RequestedPower = 1,
            Penalty = 3,
            Comment = $"Existing",
            AdditionalValueToCopy = $"ExistingAdditionalValue",
            SubEntities = Enumerable.Range(0, 5).Select(y => new EntityLevel1
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMinutes(y),
                Power = y,
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"Existing_{y}",
            }).ToList(),
        };

        var newEntity = new EntityLevel0
        {
            Index = 1,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today,
            Direction = Direction.Up,
            RequestedPower = 1,
            Penalty = 3,
            Comment = $"New",
            AdditionalValueToCopy = $"NewAdditionalValue",
            SubEntities = Enumerable.Range(1, 5).Select(y => new EntityLevel1
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMinutes(y),
                Power = y,
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"New_{y}",
            }).ToList(),
        };

        DiffConfiguration diffConfiguration = new DiffConfiguration();
        diffConfiguration.PersistEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .HasAdditionalValuesToCopy(x => new { x.AdditionalValueToCopy })
            .HasMany(x => x.SubEntities);
        diffConfiguration.PersistEntity<EntityLevel1>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power, x.Price });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var result = deepDiff.Diff(existingEntity, newEntity);

        Assert.NotNull(result);
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Insert));
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Delete));
        Assert.Equal(9, results.Count(x => x.PersistChange == PersistChange.Update));

        Assert.All(results.Where(x => x.PersistChange != PersistChange.Insert), x => Assert.StartsWith("Existing", x.Comment)); // Comment is not copied
        Assert.StartsWith("NewAdditionalValue", results.Single(x => x.PersistChange == PersistChange.Insert).AdditionalValueToCopy); // AdditionalValueToCopy is copied
    }

    // TODO: modification on level 0

    [Fact]
    public void TestMultipleChanges_MultipleEntities()
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

        DiffConfiguration diffConfiguration = new DiffConfiguration();
        diffConfiguration.PersistEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .HasAdditionalValuesToCopy(x => new { x.AdditionalValueToCopy })
            .HasMany(x => x.SubEntities);
        diffConfiguration.PersistEntity<EntityLevel1>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power, x.Price });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var results = deepDiff.Diff(existingEntities, newEntities).ToArray();

        Assert.Equal(11, results.Length);
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Insert));
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Delete));
        Assert.Equal(9, results.Count(x => x.PersistChange == PersistChange.Update));

        Assert.All(results.Where(x => x.PersistChange != PersistChange.Insert), x => Assert.StartsWith("Existing", x.Comment)); // Comment is not copied
        Assert.StartsWith("NewAdditionalValue", results.Single(x => x.PersistChange == PersistChange.Insert).AdditionalValueToCopy); // AdditionalValueToCopy is copied
    }
}
