using EntityMerger.EntityMerger;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EntityMerger.UnitTest;

public class MergerTests
{
    [Fact]
    public void Test_NoNavigation_Identical()
    {
        var existing = Enumerable.Range(0, 5).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? (decimal?)null : x * 2,
            Comment = $"Existing{x}",
        }).ToArray();

        var calculated = Enumerable.Range(0, 5).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? (decimal?)null : x * 2,
            Comment = $"Calculated{x}",
        }).ToArray();

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.Entity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValue(x => new { x.RequestedPower, x.Penalty });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Empty(results);
    }

    [Fact]
    public void Test_NoNavigation_OneDelete()
    {
        var existing = Enumerable.Range(0, 5).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? (decimal?)null : x * 2,
            Comment = $"Existing{x}",
        }).ToArray();

        // index 2 is missing -> will be marked as deleted
        var calculated = Enumerable.Range(0, 5).Except(new[] { 2 }).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? (decimal?)null : x * 2,
            Comment = $"Calculated{x}",
        }).ToArray();

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.Entity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValue(x => new { x.RequestedPower, x.Penalty });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Delete, results.Single().PersistChange);
        Assert.Same(existing.Single(x => x.Index == 2), results.Single());
    }

    [Fact]
    public void Test_NoNavigation_OneInsert()
    {
        var existing = Enumerable.Range(0, 5).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? (decimal?)null : x * 2,
            Comment = $"Existing{x}",
        }).ToArray();

        // index 5 doesn't exist in existing collection -> will be marked as inserted
        var calculated = Enumerable.Range(0, 6).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? (decimal?)null : x * 2,
            Comment = $"Calculated{x}",
        }).ToArray();

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.Entity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValue(x => new { x.RequestedPower, x.Penalty });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Insert, results.Single().PersistChange);
        Assert.Same(calculated.Single(x => x.Index == 5), results.Single());
    }

    [Fact]
    public void Test_NoNavigation_OneDelete_OneInsert()
    {
        var existing = Enumerable.Range(0, 5).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? (decimal?)null : x * 2,
            Comment = $"Existing{x}",
        }).ToArray();

        // index 2 is missing -> will be marked as deleted
        // index 5 doesn't exist in existing collection -> will be marked as inserted
        var calculated = Enumerable.Range(0, 6).Except(new[] { 2 }).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? (decimal?)null : x * 2,
            Comment = $"Calculated{x}",
        }).ToArray();

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.Entity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValue(x => new { x.RequestedPower, x.Penalty });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Equal(2, results.Length);
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Insert));
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Delete));
        Assert.Same(calculated.Single(x => x.Index == 5), results.Single(x => x.PersistChange == PersistChange.Insert));
        Assert.Same(existing.Single(x => x.Index == 2), results.Single(x => x.PersistChange == PersistChange.Delete));
    }

    [Fact]
    public void Test_NavigationMany_OneChildDelete()
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
        mergeConfiguration.Entity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValue(x => new { x.RequestedPower })
            .HasMany(x => x.SubEntities);
        mergeConfiguration.Entity<SubEntity>()
            .HasKey(x => x.Timestamp)
            .HasValue(x => new { x.Power });

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
    public void Test_NavigationMany_OneChildInsert()
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
        mergeConfiguration.Entity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValue(x => new { x.RequestedPower })
            .HasMany(x => x.SubEntities);
        mergeConfiguration.Entity<SubEntity>()
            .HasKey(x => x.Timestamp)
            .HasValue(x => new { x.Power });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Single(results);
        Assert.Equal(PersistChange.Update, results.Single().PersistChange);
        Assert.Same(existing.Single(), results.Single());
        Assert.Single(results.Single().SubEntities);
        Assert.Equal(PersistChange.Insert, results.Single().SubEntities.Single().PersistChange);
        Assert.Same(calculated.Single().SubEntities.Single(), results.Single().SubEntities.Single());
    }

    [Fact]
    public void Test1()
    {
        var existing = Enumerable.Range(0, 10).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? (decimal?)null : x * 2,
            Comment = $"Existing{x}",
            SubEntities = Enumerable.Range(0, 5).Select(y => new SubEntity
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddHours(x).AddMinutes(y),
                Power = y,
                Price = y % 2 == 0 ? (decimal?)null : y * 3,
                Comment = $"Existing{x}_{y}",
            }).ToList(),
        }).ToArray();

        var calculated = Enumerable.Range(1, 10).Select(x => new Entity
        {
            Index = x,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddHours(x),
            Direction = x % 2 == 0 ? Direction.Up : Direction.Down,
            RequestedPower = x,
            Penalty = x % 3 == 0 ? (decimal?)null : x * 3,
            Comment = $"Calculated{x}",
            SubEntities = Enumerable.Range(1, 5).Select(y => new SubEntity
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddHours(x).AddMinutes(y),
                Power = y,
                Price = y % 2 == 0 ? (decimal?)null : y * 3,
                Comment = $"Calculated{x}_{y}",
            }).ToList(),
        }).ToArray();

        MergeConfiguration mergeConfiguration = new MergeConfiguration();
        mergeConfiguration.Entity<Entity>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValue(x => new { x.RequestedPower, x.Penalty })
            .HasMany(x => x.SubEntities);
        mergeConfiguration.Entity<SubEntity>()
            .HasKey(x => x.Timestamp)
            .HasValue(x => new { x.Power, x.Price });

        var merger = mergeConfiguration.CreateMerger();
        var results = merger.Merge(existing, calculated).ToArray();

        Assert.Equal(11, results.Length);
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Insert));
        Assert.Equal(1, results.Count(x => x.PersistChange == PersistChange.Delete));
        Assert.Equal(9, results.Count(x => x.PersistChange == PersistChange.Update));
    }

    //
    internal class Entity : PersistEntity
    {
        public Guid Id { get; set; }
        public DateTime StartsOn { get; set; }
        public Direction Direction { get; set; }

        public decimal RequestedPower { get; set; }
        public decimal? Penalty { get; set; }

        public string Comment { get; set; }

        public List<SubEntity> SubEntities { get; set; }

        // debug property, will never participate in merge neither as key nor value
        public int Index { get; set; }

    }

    internal class SubEntity : PersistEntity
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }

        public decimal Power { get; set; }
        public decimal? Price { get; set; }

        public string Comment { get; set; }

        // debug property, will never participate in merge neither as key nor value
        public int Index { get; set; }
    }
}
