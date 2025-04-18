using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Simple;

public class SimpleEntityNavigationManyTests
{
    [Theory]
    [InlineData(EqualityComparers.Precompiled)]
    [InlineData(EqualityComparers.Naive)]
    public void OneChildDelete(EqualityComparers equalityComparer)
    {
        var existingEntities = new[]
        {
            new EntityLevel0
            {
                Index = 0,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,
                RequestedPower = 1,

                SubEntities = new List<EntityLevel1>
                {
                    new EntityLevel1
                    {
                        Index = 0,

                        Timestamp = DateTime.Today,
                        Power = 1
                    }
                }
            }
        };

        var newEntities = new[]
        {
            new EntityLevel0
            {
                Index = 0,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,
                RequestedPower = 1,

                SubEntities = new List<EntityLevel1>()
            }
        };

        var diffConfiguration = new DeepDiffConfiguration();
        diffConfiguration.ConfigureEntity<EntityLevel0>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower })
            .HasMany(x => x.SubEntities);
        diffConfiguration.ConfigureEntity<EntityLevel1>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var results = deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.SetEqualityComparer(equalityComparer)).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Same(existingEntities.Single(), results.Single());
        Assert.Single(results.Single().SubEntities);
        Assert.Equal(PersistChange.Delete, results.Single().SubEntities.Single().PersistChange);
        Assert.Same(existingEntities.Single().SubEntities.Single(), results.Single().SubEntities.Single());
    }

    [Theory]
    [InlineData(EqualityComparers.Precompiled)]
    [InlineData(EqualityComparers.Naive)]
    public void OneChildInsert(EqualityComparers equalityComparer)
    {
        var existingEntities = new[]
        {
            new EntityLevel0
            {
                Index = 0,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,
                RequestedPower = 1,

                SubEntities = new List<EntityLevel1>()
            }
        };

        var newEntities = new[]
        {
            new EntityLevel0
            {
                Index = 0,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,
                RequestedPower = 1,

                SubEntities = new List<EntityLevel1>
                {
                    new EntityLevel1
                    {
                        Index = 0,

                        Timestamp = DateTime.Today,
                        Power = 1
                    }
                }
            }
        };

        DeepDiffConfiguration diffConfiguration = new DeepDiffConfiguration();
        diffConfiguration.ConfigureEntity<EntityLevel0>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower })
            .HasMany(x => x.SubEntities);
        diffConfiguration.ConfigureEntity<EntityLevel1>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var results = deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.SetEqualityComparer(equalityComparer)).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Same(existingEntities.Single(), results.Single());
        Assert.Single(results.Single().SubEntities);
        Assert.Equal(PersistChange.Insert, results.Single().SubEntities.Single().PersistChange);
        Assert.Same(newEntities.Single().SubEntities.Single(), results.Single().SubEntities.Single());
    }
}
