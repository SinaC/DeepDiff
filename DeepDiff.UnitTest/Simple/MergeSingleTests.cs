using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Simple;

public class MergeSingleTests
{
    [Theory]
    [InlineData(EqualityComparers.Precompiled)]
    [InlineData(EqualityComparers.Naive)]
    public void ModificationsInNested(EqualityComparers equalityComparer)
    {
        var existingEntity = new EntityLevel0
        {
            Index = 1,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today,
            Direction = Direction.Up,
            RequestedPower = 1,
            Penalty = 3,
            Comment = $"Existing",
            AdditionalValueToCopy = $"ExistingAdditionalValue",
            SubEntities = Enumerable.Range(0, 5).Select(y => new EntityLevel1
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMinutes(y),
                Power = y,
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"Existing_{y}",
            }).ToList(),
        };

        var newEntity = new EntityLevel0
        {
            Index = 1,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today,
            Direction = Direction.Up,
            RequestedPower = 1,
            Penalty = 3,
            Comment = $"New",
            AdditionalValueToCopy = $"NewAdditionalValue",
            SubEntities = Enumerable.Range(1, 5).Select(y => new EntityLevel1
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMinutes(y),
                Power = 2 * y,
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"New_{y}",
            }).ToList(),
        };

        DeepDiffConfiguration diffConfiguration = new DeepDiffConfiguration();
        diffConfiguration.Entity<EntityLevel0>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
            .HasMany(x => x.SubEntities);
        diffConfiguration.Entity<EntityLevel1>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power, x.Price });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var result = deepDiff.MergeSingle(existingEntity, newEntity, cfg => cfg.SetEqualityComparer(equalityComparer));

        Assert.NotNull(result);
        Assert.Equal(PersistChange.None, result.PersistChange);
        Assert.Single(result.SubEntities.Where(x => x.PersistChange == PersistChange.Insert));
        Assert.Single(result.SubEntities.Where(x => x.PersistChange == PersistChange.Delete));
        Assert.Equal(4, result.SubEntities.Where(x => x.PersistChange == PersistChange.Update).Count());
    }

    [Theory]
    [InlineData(EqualityComparers.Precompiled)]
    [InlineData(EqualityComparers.Naive)]
    public void ModificationsInNested_ForceOnUpdate_Precompiled(EqualityComparers equalityComparer)
    {
        var existingEntity = new EntityLevel0
        {
            Index = 1,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today,
            Direction = Direction.Up,
            RequestedPower = 1,
            Penalty = 3,
            Comment = $"Existing",
            AdditionalValueToCopy = $"ExistingAdditionalValue",
            SubEntities = Enumerable.Range(0, 5).Select(y => new EntityLevel1
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMinutes(y),
                Power = y,
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"Existing_{y}",
            }).ToList(),
        };

        var newEntity = new EntityLevel0
        {
            Index = 1,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today,
            Direction = Direction.Up,
            RequestedPower = 1,
            Penalty = 3,
            Comment = $"New",
            AdditionalValueToCopy = $"NewAdditionalValue",
            SubEntities = Enumerable.Range(1, 5).Select(y => new EntityLevel1
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMinutes(y),
                Power = 2 * y,
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"New_{y}",
            }).ToList(),
        };

        DeepDiffConfiguration diffConfiguration = new DeepDiffConfiguration();
        diffConfiguration.Entity<EntityLevel0>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
            .HasMany(x => x.SubEntities);
        diffConfiguration.Entity<EntityLevel1>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power, x.Price });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var result = deepDiff.MergeSingle(existingEntity, newEntity, cfg => cfg.ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(true).SetEqualityComparer(equalityComparer));

        Assert.NotNull(result);
        Assert.Equal(PersistChange.Update, result.PersistChange);
        Assert.Single(result.SubEntities.Where(x => x.PersistChange == PersistChange.Insert));
        Assert.Single(result.SubEntities.Where(x => x.PersistChange == PersistChange.Delete));
        Assert.Equal(4, result.SubEntities.Where(x => x.PersistChange == PersistChange.Update).Count());
    }

    [Theory]
    [InlineData(EqualityComparers.Precompiled)]
    [InlineData(EqualityComparers.Naive)]
    public void ModificationsOnRoot_Precompiled(EqualityComparers equalityComparer)
    {
        var existingEntity = new EntityLevel0
        {
            Index = 1,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today,
            Direction = Direction.Up,
            RequestedPower = 1,
            Penalty = 3,
            Comment = $"Existing",
            AdditionalValueToCopy = $"ExistingAdditionalValue",
            SubEntities = Enumerable.Range(0, 5).Select(y => new EntityLevel1
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMinutes(y),
                Power = y,
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"Existing_{y}",
            }).ToList(),
        };

        var newEntity = new EntityLevel0
        {
            Index = 1,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today,
            Direction = Direction.Up,
            RequestedPower = 1,
            Penalty = 7,
            Comment = $"New",
            AdditionalValueToCopy = $"NewAdditionalValue",
            SubEntities = Enumerable.Range(0, 5).Select(y => new EntityLevel1
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMinutes(y),
                Power = y,
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"Existing_{y}",
            }).ToList(),
        };

        DeepDiffConfiguration diffConfiguration = new DeepDiffConfiguration();
        diffConfiguration.Entity<EntityLevel0>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
            .HasMany(x => x.SubEntities);
        diffConfiguration.Entity<EntityLevel1>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power, x.Price });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var result = deepDiff.MergeSingle(existingEntity, newEntity, cfg => cfg.SetEqualityComparer(equalityComparer));

        Assert.NotNull(result);
        Assert.Equal(PersistChange.Update, result.PersistChange);
        Assert.Equal(7, result.Penalty);
        Assert.Equal("NewAdditionalValue", result.AdditionalValueToCopy);
        Assert.Equal("Existing", result.Comment);
        Assert.Empty(result.SubEntities);
    }

    [Theory]
    [InlineData(EqualityComparers.Precompiled)]
    [InlineData(EqualityComparers.Naive)]
    public void DifferentKey_Precompiled(EqualityComparers equalityComparer)
    {
        var existingEntity = new EntityLevel0
        {
            Index = 1,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today,
            Direction = Direction.Up,
            RequestedPower = 1,
            Penalty = 3,
            Comment = $"Existing",
            AdditionalValueToCopy = $"ExistingAdditionalValue",
            SubEntities = Enumerable.Range(0, 5).Select(y => new EntityLevel1
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMinutes(y),
                Power = y,
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"Existing_{y}",
            }).ToList(),
        };

        var newEntity = new EntityLevel0
        {
            Index = 1,

            Id = Guid.NewGuid(),
            StartsOn = DateTime.Today.AddDays(1),
            Direction = Direction.Up,
            RequestedPower = 1,
            Penalty = 3,
            Comment = $"NewExisting",
            AdditionalValueToCopy = $"NewAdditionalValue",
            SubEntities = Enumerable.Range(0, 5).Select(y => new EntityLevel1
            {
                Index = y,

                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMinutes(y),
                Power = y,
                Price = y % 2 == 0 ? null : y * 3,
                Comment = $"Existing_{y}",
            }).ToList(),
        };

        DeepDiffConfiguration diffConfiguration = new DeepDiffConfiguration();
        diffConfiguration.Entity<EntityLevel0>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
            .HasMany(x => x.SubEntities);
        diffConfiguration.Entity<EntityLevel1>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power, x.Price });

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var result = deepDiff.MergeSingle(existingEntity, newEntity, cfg => cfg.SetEqualityComparer(equalityComparer));

        Assert.NotNull(result);
        Assert.Equal(PersistChange.Update, result.PersistChange);
        Assert.Equal(DateTime.Today.AddDays(1), result.StartsOn);
        Assert.Equal(1, result.RequestedPower);
        Assert.Equal(3, result.Penalty);
        Assert.Equal("NewAdditionalValue", result.AdditionalValueToCopy);
        Assert.Equal("Existing", result.Comment);
        Assert.Empty(result.SubEntities);
    }
}
