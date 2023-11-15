using EntityMerger.EntityMerger;
using EntityMerger.UnitTest.Entities;
using EntityMerger.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace EntityMerger.UnitTest.Simple;

public class SimpleEntityNavigationOneTests
{
    [Fact]
    public void NotInExisting()
    {
        var existing = new[]
        {
            new Entity
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,
            }
        };

        var calculated = new[]
        {
            new Entity
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new SubEntity
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(15),

                    Power = 1,
                }
            }
        };

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.PersistEntity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasCalculatedValue(x => new { x.RequestedPower, x.Penalty })
            .HasOne(x => x.SubEntity);
        mergeConfiguration.PersistEntity<SubEntity>()
            .HasKey(x => x.Timestamp)
            .HasCalculatedValue(x => new { x.Power });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Update, results.Single().PersistChange);
        Assert.Equal(PersistChange.Insert, results.Single().SubEntity.PersistChange);
    }

    [Fact]
    public void NotInCalculated()
    {
        var existing = new[]
        {
            new Entity
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new SubEntity
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(15),

                    Power = 1,
                }
            }
        };

        var calculated = new[]
        {
            new Entity
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1
            }
        };

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.PersistEntity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasCalculatedValue(x => new { x.RequestedPower, x.Penalty })
            .HasOne(x => x.SubEntity);
        mergeConfiguration.PersistEntity<SubEntity>()
            .HasKey(x => x.Timestamp)
            .HasCalculatedValue(x => new { x.Power });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Update, results.Single().PersistChange);
        Assert.Equal(PersistChange.Delete, results.Single().SubEntity.PersistChange);
    }

    [Fact]
    public void Identical()
    {
        var existing = new[]
        {
            new Entity
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new SubEntity
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(15),

                    Power = 1,
                }
            }
        };

        var calculated = new[]
        {
            new Entity
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new SubEntity
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(15),

                    Power = 1,
                }
            }
        };

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.PersistEntity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasCalculatedValue(x => new { x.RequestedPower, x.Penalty })
            .HasOne(x => x.SubEntity);
        mergeConfiguration.PersistEntity<SubEntity>()
            .HasKey(x => x.Timestamp)
            .HasCalculatedValue(x => new { x.Power });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Empty(results);
    }

    [Fact]
    public void KeyDifferent()
    {
        var existing = new[]
        {
            new Entity
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new SubEntity
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(15),

                    Power = 1,
                }
            }
        };

        var calculated = new[]
        {
            new Entity
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new SubEntity
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(30),

                    Power = 1,
                }
            }
        };

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.PersistEntity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasCalculatedValue(x => new { x.RequestedPower, x.Penalty })
            .HasOne(x => x.SubEntity);
        mergeConfiguration.PersistEntity<SubEntity>()
            .HasKey(x => x.Timestamp)
            .HasCalculatedValue(x => new { x.Power });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Update, results.Single().PersistChange);
        Assert.Equal(PersistChange.Update, results.Single().SubEntity.PersistChange);
        Assert.Equal(calculated.Single().SubEntity.Timestamp, results.Single().SubEntity.Timestamp);
    }

    [Fact]
    public void ValueDifferent()
    {
        var existing = new[]
        {
            new Entity
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new SubEntity
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(15),

                    Power = 1,
                }
            }
        };

        var calculated = new[]
        {
            new Entity
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new SubEntity
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(15),

                    Power = 30,
                }
            }
        };

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.PersistEntity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasCalculatedValue(x => new { x.RequestedPower, x.Penalty })
            .HasOne(x => x.SubEntity);
        mergeConfiguration.PersistEntity<SubEntity>()
            .HasKey(x => x.Timestamp)
            .HasCalculatedValue(x => new { x.Power });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Update, results.Single().PersistChange);
        Assert.Equal(PersistChange.Update, results.Single().SubEntity.PersistChange);
        Assert.Equal(calculated.Single().SubEntity.Power, results.Single().SubEntity.Power);
    }
}
