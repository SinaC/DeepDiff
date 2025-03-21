using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Simple;

public class SimpleEntityNavigationOneTests
{
    [Theory]
    [InlineData(EqualityComparers.Precompiled, true)]
    [InlineData(EqualityComparers.Precompiled, false)]
    [InlineData(EqualityComparers.Naive, true)]
    [InlineData(EqualityComparers.Naive, false)]
    public void NotInExisting(EqualityComparers equalityComparer, bool useParallelism)
    {
        var existingEntities = new[]
        {
            new EntityLevel0
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,
            }
        };

        var newEntities = new[]
        {
            new EntityLevel0
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new EntityLevel1
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(15),

                    Power = 1,
                }
            }
        };

        var deepDiff = CreateDeepDiff();
        var results = deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.SetEqualityComparer(equalityComparer).UseParallelism(useParallelism));

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Equal(PersistChange.Insert, results.Single().SubEntity.PersistChange);
    }

    [Theory]
    [InlineData(EqualityComparers.Precompiled, true)]
    [InlineData(EqualityComparers.Precompiled, false)]
    [InlineData(EqualityComparers.Naive, true)]
    [InlineData(EqualityComparers.Naive, false)]
    public void NotInNew(EqualityComparers equalityComparer, bool useParallelism)
    {
        var existingEntities = new[]
        {
            new EntityLevel0
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new EntityLevel1
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(15),

                    Power = 1,
                }
            }
        };

        var newEntities = new[]
        {
            new EntityLevel0
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1
            }
        };

        var deepDiff = CreateDeepDiff();
        var results = deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.SetEqualityComparer(equalityComparer).UseParallelism(useParallelism));

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Equal(PersistChange.Delete, results.Single().SubEntity.PersistChange);
    }

    [Theory]
    [InlineData(EqualityComparers.Precompiled, true)]
    [InlineData(EqualityComparers.Precompiled, false)]
    [InlineData(EqualityComparers.Naive, true)]
    [InlineData(EqualityComparers.Naive, false)]
    public void Identical(EqualityComparers equalityComparer, bool useParallelism)
    {
        var existingEntities = new[]
        {
            new EntityLevel0
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new EntityLevel1
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(15),

                    Power = 1,
                }
            }
        };

        var newEntities = new[]
        {
            new EntityLevel0
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new EntityLevel1
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(15),

                    Power = 1,
                }
            }
        };

        var deepDiff = CreateDeepDiff();
        var results = deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.SetEqualityComparer(equalityComparer).UseParallelism(useParallelism));

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(EqualityComparers.Precompiled, true)]
    [InlineData(EqualityComparers.Precompiled, false)]
    [InlineData(EqualityComparers.Naive, true)]
    [InlineData(EqualityComparers.Naive, false)]
    public void KeyDifferent(EqualityComparers equalityComparer, bool useParallelism)
    {
        var existingEntities = new[]
        {
            new EntityLevel0
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new EntityLevel1
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(15),

                    Power = 1,
                }
            }
        };

        var newEntities = new[]
        {
            new EntityLevel0
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new EntityLevel1
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(30),

                    Power = 1,
                }
            }
        };

        var deepDiff = CreateDeepDiff();
        var results = deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.SetEqualityComparer(equalityComparer).UseParallelism(useParallelism));

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Equal(PersistChange.Update, results.Single().SubEntity.PersistChange);
        Assert.Equal(newEntities.Single().SubEntity.Timestamp, results.Single().SubEntity.Timestamp);
    }


    [Theory]
    [InlineData(EqualityComparers.Precompiled, true)]
    [InlineData(EqualityComparers.Precompiled, false)]
    [InlineData(EqualityComparers.Naive, true)]
    [InlineData(EqualityComparers.Naive, false)]
    public void ValueDifferent(EqualityComparers equalityComparer, bool useParallelism)
    {
        var existingEntities = new[]
        {
            new EntityLevel0
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new EntityLevel1
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(15),

                    Power = 1,
                }
            }
        };

        var newEntities = new[]
        {
            new EntityLevel0
            {
                Index = 1,

                StartsOn = DateTime.Today,
                Direction = Direction.Up,

                RequestedPower = 1,
                Penalty = 1,

                SubEntity = new EntityLevel1
                {
                    Index = 1,

                    Timestamp = DateTime.Today.AddMinutes(15),

                    Power = 30,
                }
            }
        };

        var deepDiff = CreateDeepDiff();
        var results = deepDiff.MergeMany(existingEntities, newEntities, cfg => cfg.SetEqualityComparer(equalityComparer).UseParallelism(useParallelism));

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Equal(PersistChange.Update, results.Single().SubEntity.PersistChange);
        Assert.Equal(newEntities.Single().SubEntity.Power, results.Single().SubEntity.Power);
    }

    [Theory]
    [InlineData(EqualityComparers.Precompiled, true)]
    [InlineData(EqualityComparers.Precompiled, false)]
    [InlineData(EqualityComparers.Naive, true)]
    [InlineData(EqualityComparers.Naive, false)]
    public void InsertHasOneNoModifOnRoot(EqualityComparers equalityComparer, bool useParallelism)
    {
        var existingEntity = new EntityLevel0
        {
            StartsOn = DateTime.Today,
            Direction = Direction.Up,
            RequestedPower = 10m,
            Penalty = 5m,
            SubEntity = null!
        };

        var calculatedEntity = new EntityLevel0
        {
            StartsOn = DateTime.Today,
            Direction = Direction.Up,
            RequestedPower = 10m,
            Penalty = 5m,
            AdditionalValueToCopy = "SHOULD BE COPIED",
            SubEntity = new EntityLevel1
            {
                Timestamp = DateTime.Today,
                Power = 11m,
                Price = 12m
            }
        };

        var deepDiff = CreateDeepDiff();
        var result = deepDiff.MergeSingle(existingEntity, calculatedEntity, cfg => cfg.ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(true).SetEqualityComparer(equalityComparer).UseParallelism(useParallelism));

        Assert.NotNull(result);
        Assert.Equal(PersistChange.Update, result.PersistChange);
        Assert.Equal("SHOULD BE COPIED", result.AdditionalValueToCopy);
        Assert.NotNull(result.SubEntity);
        Assert.Equal(PersistChange.Insert, result.SubEntity.PersistChange);
    }

    private static IDeepDiff CreateDeepDiff()
    {
        var diffConfiguration = new DeepDiffConfiguration();
        diffConfiguration.Entity<EntityLevel0>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update).CopyValues(x => x.AdditionalValueToCopy))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .HasOne(x => x.SubEntity);
        diffConfiguration.Entity<EntityLevel1>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power });
        return diffConfiguration.CreateDeepDiff();
    }
}
