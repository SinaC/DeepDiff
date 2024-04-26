using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Simple;

public class SimpleEntityNavigationOneTests
{
    [Fact]
    public void NotInExisting()
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
        var diff = deepDiff.DiffMany(existingEntities, newEntities);
        var results = diff.Entities.ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Equal(PersistChange.Insert, results.Single().SubEntity.PersistChange);
    }

    [Fact]
    public void NotInExisting_Naive()
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
        var diff = deepDiff.DiffMany(existingEntities, newEntities, cfg => cfg.DisablePrecompiledEqualityComparer());
        var results = diff.Entities.ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Equal(PersistChange.Insert, results.Single().SubEntity.PersistChange);
    }

    [Fact]
    public void NotInNew()
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
        var diff = deepDiff.DiffMany(existingEntities, newEntities);
        var results = diff.Entities.ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Equal(PersistChange.Delete, results.Single().SubEntity.PersistChange);
    }

    [Fact]
    public void NotInNew_Naive()
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
        var diff = deepDiff.DiffMany(existingEntities, newEntities, cfg => cfg.DisablePrecompiledEqualityComparer());
        var results = diff.Entities.ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Equal(PersistChange.Delete, results.Single().SubEntity.PersistChange);
    }

    [Fact]
    public void Identical()
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
        var diff = deepDiff.DiffMany(existingEntities, newEntities);
        var results = diff.Entities.ToArray();

        Assert.Empty(results);
    }

    [Fact]
    public void Identical_Naive()
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
        var diff = deepDiff.DiffMany(existingEntities, newEntities, cfg => cfg.DisablePrecompiledEqualityComparer());
        var results = diff.Entities.ToArray();

        Assert.Empty(results);
    }

    [Fact]
    public void KeyDifferent()
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
        var diff = deepDiff.DiffMany(existingEntities, newEntities);
        var results = diff.Entities.ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Equal(PersistChange.Update, results.Single().SubEntity.PersistChange);
        Assert.Equal(newEntities.Single().SubEntity.Timestamp, results.Single().SubEntity.Timestamp);
    }

    [Fact]
    public void KeyDifferent_Naive()
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
        var diff = deepDiff.DiffMany(existingEntities, newEntities, cfg => cfg.DisablePrecompiledEqualityComparer());
        var results = diff.Entities.ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Equal(PersistChange.Update, results.Single().SubEntity.PersistChange);
        Assert.Equal(newEntities.Single().SubEntity.Timestamp, results.Single().SubEntity.Timestamp);
    }

    [Fact]
    public void ValueDifferent()
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
        var diff = deepDiff.DiffMany(existingEntities, newEntities);
        var results = diff.Entities.ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Equal(PersistChange.Update, results.Single().SubEntity.PersistChange);
        Assert.Equal(newEntities.Single().SubEntity.Power, results.Single().SubEntity.Power);
    }

    [Fact]
    public void ValueDifferent_Naive()
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
        var diff = deepDiff.DiffMany(existingEntities, newEntities, cfg => cfg.DisablePrecompiledEqualityComparer());
        var results = diff.Entities.ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.None, results.Single().PersistChange);
        Assert.Equal(PersistChange.Update, results.Single().SubEntity.PersistChange);
        Assert.Equal(newEntities.Single().SubEntity.Power, results.Single().SubEntity.Power);
    }

    //[Fact(Skip = "Wait issue 69")]
    [Fact]
    public void InsertHasOneNoModifOnRoot()
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
        var diff = deepDiff.DiffSingle(existingEntity, calculatedEntity, cfg => cfg.ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel()); // TODO: will be replaced with issue 69

        Assert.NotNull(diff.Entity);
        Assert.Equal(PersistChange.Update, diff.Entity.PersistChange);
        Assert.Equal("SHOULD BE COPIED", diff.Entity.AdditionalValueToCopy);
        Assert.NotNull(diff.Entity.SubEntity);
        Assert.Equal(PersistChange.Insert, diff.Entity.SubEntity.PersistChange);
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
