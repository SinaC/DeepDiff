using EntityMerger.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using EntityMerger.UnitTest.Entities;
using EntityMerger.UnitTest.Entities.Simple;

namespace EntityMerger.UnitTest.Simple;

public class SimpleEntityNavigationManyTests
{
    [Fact]
    public void OneChildDelete()
    {
        var existingEntities = new[]
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

        var newEntities = new[]
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
            .HasValues(x => new { x.RequestedPower })
            .HasMany(x => x.SubEntities);
        mergeConfiguration.PersistEntity<SubEntity>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existingEntities, newEntities).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Update, results.Single().PersistChange);
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
            new Entity
            {
                Index = 0,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,
                RequestedPower = 1,

                SubEntities = new List<SubEntity>()
            }
        };

        var newEntities = new[]
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
            .HasValues(x => new { x.RequestedPower })
            .HasMany(x => x.SubEntities);
        mergeConfiguration.PersistEntity<SubEntity>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existingEntities, newEntities).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Update, results.Single().PersistChange);
        Assert.Same(existingEntities.Single(), results.Single());
        Assert.Single(results.Single().SubEntities);
        Assert.Equal(PersistChange.Insert, results.Single().SubEntities.Single().PersistChange);
        Assert.Same(newEntities.Single().SubEntities.Single(), results.Single().SubEntities.Single());
    }
}
