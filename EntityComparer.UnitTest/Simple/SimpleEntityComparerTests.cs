using EntityComparer.Configuration;
using EntityComparer.UnitTest.Entities;
using EntityComparer.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace EntityComparer.UnitTest.Simple;

public partial class SimpleEntityComparerTests
{
    [Fact]
    public void TestMultipleChanges()
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

        CompareConfiguration compareConfiguration = new CompareConfiguration();
        compareConfiguration.PersistEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .HasAdditionalValuesToCopy(x => new { x.AdditionalValueToCopy })
            .HasMany(x => x.SubEntities);
        compareConfiguration.PersistEntity<EntityLevel1>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power, x.Price });

        var comparer = compareConfiguration.CreateComparer();
        var results = comparer.Compare(existingEntities, newEntities).ToArray();

        Assert.Equal(11, results.Length);
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Insert));
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Delete));
        Assert.Equal(9, results.Count(x => x.PersistChange == PersistChange.Update));

        Assert.All(results.Where(x => x.PersistChange != PersistChange.Insert), x => Assert.StartsWith("Existing", x.Comment)); // Comment is not copied
        Assert.StartsWith("NewAdditionalValue", results.Single(x => x.PersistChange == PersistChange.Insert).AdditionalValueToCopy); // AdditionalValueToCopy is copied
    }
}