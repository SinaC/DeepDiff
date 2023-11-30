using EntityComparer.Configuration;
using EntityComparer.UnitTest.Entities;
using EntityComparer.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace EntityComparer.UnitTest.Simple;

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

        CompareConfiguration compareConfiguration = new CompareConfiguration();
        compareConfiguration.PersistEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .HasOne(x => x.SubEntity);
        compareConfiguration.PersistEntity<EntityLevel1>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power });

        var comparer = compareConfiguration.CreateComparer();
        var results = comparer.Compare(existingEntities, newEntities).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Update, results.Single().PersistChange);
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

        CompareConfiguration compareConfiguration = new CompareConfiguration();
        compareConfiguration.PersistEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .HasOne(x => x.SubEntity);
        compareConfiguration.PersistEntity<EntityLevel1>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power });

        var comparer = compareConfiguration.CreateComparer();
        var results = comparer.Compare(existingEntities, newEntities).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Update, results.Single().PersistChange);
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

        CompareConfiguration compareConfiguration = new CompareConfiguration();
        compareConfiguration.PersistEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .HasOne(x => x.SubEntity);
        compareConfiguration.PersistEntity<EntityLevel1>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power });

        var comparer = compareConfiguration.CreateComparer();
        var results = comparer.Compare(existingEntities, newEntities).ToArray();

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

        CompareConfiguration compareConfiguration = new CompareConfiguration();
        compareConfiguration.PersistEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .HasOne(x => x.SubEntity);
        compareConfiguration.PersistEntity<EntityLevel1>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power });

        var comparer = compareConfiguration.CreateComparer();
        var results = comparer.Compare(existingEntities, newEntities).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Update, results.Single().PersistChange);
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

        CompareConfiguration compareConfiguration = new CompareConfiguration();
        compareConfiguration.PersistEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .HasOne(x => x.SubEntity);
        compareConfiguration.PersistEntity<EntityLevel1>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power });

        var comparer = compareConfiguration.CreateComparer();
        var results = comparer.Compare(existingEntities, newEntities).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Update, results.Single().PersistChange);
        Assert.Equal(PersistChange.Update, results.Single().SubEntity.PersistChange);
        Assert.Equal(newEntities.Single().SubEntity.Power, results.Single().SubEntity.Power);
    }
}