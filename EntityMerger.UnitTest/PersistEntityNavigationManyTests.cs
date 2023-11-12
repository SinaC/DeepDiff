using EntityMerger.EntityMerger;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using EntityMerger.UnitTest.Entities;

namespace EntityMerger.UnitTest;

public class PersistEntityNavigationManyTests
{
    [Fact]
    public void OneChildDelete()
    {
        var existing = new[]
        {
            new Entity
            {
                Index = 0,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,
                RequestedPower = 1,

                SubEntities = new List<SubEntity>
                {
                    new SubEntity
                    {
                        Index = 0,

                        Timestamp = DateTime.Today,
                        Power = 1
                    }
                }
            }
        };

        var calculated = new[]
        {
            new Entity
            {
                Index = 0,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,
                RequestedPower = 1,

                SubEntities = new List<SubEntity>()
            }
        };

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.PersistEntity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasCalculatedValue(x => new { x.RequestedPower })
            .HasMany(x => x.SubEntities);
        mergeConfiguration.PersistEntity<SubEntity>()
            .HasKey(x => x.Timestamp)
            .HasCalculatedValue(x => new { x.Power });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Update, results.Single().PersistChange);
        Assert.Same(existing.Single(), results.Single());
        Assert.Single(results.Single().SubEntities);
        Assert.Equal(PersistChange.Delete, results.Single().SubEntities.Single().PersistChange);
        Assert.Same(existing.Single().SubEntities.Single(), results.Single().SubEntities.Single());
    }

    [Fact]
    public void OneChildInsert()
    {
        var existing = new[]
        {
            new Entity
            {
                Index = 0,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,
                RequestedPower = 1,

                SubEntities = new List<SubEntity>()
            }
        };

        var calculated = new[]
        {
            new Entity
            {
                Index = 0,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,
                RequestedPower = 1,

                SubEntities = new List<SubEntity>
                {
                    new SubEntity
                    {
                        Index = 0,

                        Timestamp = DateTime.Today,
                        Power = 1
                    }
                }
            }
        };

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.PersistEntity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasCalculatedValue(x => new { x.RequestedPower })
            .HasMany(x => x.SubEntities);
        mergeConfiguration.PersistEntity<SubEntity>()
            .HasKey(x => x.Timestamp)
            .HasCalculatedValue(x => new { x.Power });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Update, results.Single().PersistChange);
        Assert.Same(existing.Single(), results.Single());
        Assert.Single(results.Single().SubEntities);
        Assert.Equal(PersistChange.Insert, results.Single().SubEntities.Single().PersistChange);
        Assert.Same(calculated.Single().SubEntities.Single(), results.Single().SubEntities.Single());
    }
}
