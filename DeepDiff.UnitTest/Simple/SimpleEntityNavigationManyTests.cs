using DeepDiff.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;

namespace DeepDiff.UnitTest.Simple;

public class SimpleEntityNavigationManyTests
{
    [Fact]
    public void OneChildDelete()
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

        var diffConfiguration = new DiffConfiguration();
        diffConfiguration.PersistEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower })
            .HasMany(x => x.SubEntities);
        diffConfiguration.PersistEntity<EntityLevel1>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var results = deepDiff.DiffMany(existingEntities, newEntities).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Same(existingEntities.Single(), results.Single());
        Assert.Single(results.Single().SubEntities);
        Assert.Equal(PersistChange.Delete, results.Single().SubEntities.Single().PersistChange);
        Assert.Same(existingEntities.Single().SubEntities.Single(), results.Single().SubEntities.Single());
    }

    [Fact]
    public void OneChildInsert()
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

        DiffConfiguration diffConfiguration = new DiffConfiguration();
        diffConfiguration.PersistEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower })
            .HasMany(x => x.SubEntities);
        diffConfiguration.PersistEntity<EntityLevel1>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var results = deepDiff.DiffMany(existingEntities, newEntities).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Same(existingEntities.Single(), results.Single());
        Assert.Single(results.Single().SubEntities);
        Assert.Equal(PersistChange.Insert, results.Single().SubEntities.Single().PersistChange);
        Assert.Same(newEntities.Single().SubEntities.Single(), results.Single().SubEntities.Single());
    }
}
