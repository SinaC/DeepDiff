using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Simple;

public class SimpleEntityNoConfigTests
{
    [Theory]
    [InlineData(EqualityComparers.Precompiled)]
    [InlineData(EqualityComparers.Naive)]
    public void NoUpdateInsertDeleteConfig(EqualityComparers equalityComparer)
    {
        var (existingEntity, newEntity) = GenerateEntities();

        //
        var diffConfiguration = new DeepDiffConfiguration();
        diffConfiguration.ConfigureEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .HasMany(x => x.SubEntities);
            //.OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            //.OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            //.OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));
        diffConfiguration.ConfigureEntity<EntityLevel1>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power, x.Price });
            //.OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            //.OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            //.OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var listener = new StoreAllOperationListener();
        var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
        var operations = listener.Operations;

        // 1 insert, 1 delete, 4 updates -> not detected because no OnInsert/OnDelete/OnUpdate specified
        Assert.NotNull(result);
        Assert.Equal(PersistChange.None, result.PersistChange);
        Assert.All(result.SubEntities, x => Assert.Equal(PersistChange.None, x.PersistChange));
        // operations are still detected even if not configured
        Assert.Equal(6, operations.Count);
        Assert.Single(operations.OfType<DeleteDiffOperation>());
        Assert.Single(operations.OfType<InsertDiffOperation>());
        Assert.Equal(4, operations.OfType<UpdateDiffOperation>().Count());
    }

    [Theory]
    [InlineData(EqualityComparers.Precompiled)]
    [InlineData(EqualityComparers.Naive)]
    public void NoUpdateConfig(EqualityComparers equalityComparer)
    {
        var (existingEntity, newEntity) = GenerateEntities();

        //
        var diffConfiguration = new DeepDiffConfiguration();
        diffConfiguration.ConfigureEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .HasMany(x => x.SubEntities)
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            //.OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));
        diffConfiguration.ConfigureEntity<EntityLevel1>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power, x.Price })
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            //.OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var listener = new StoreAllOperationListener();
        var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
        var operations = listener.Operations;

        // 1 insert, 1 delete, 4 updates -> insert and deleted will be detected
        Assert.NotNull(result);
        Assert.Equal(PersistChange.None, result.PersistChange);
        Assert.All(result.SubEntities, x => Assert.NotEqual(PersistChange.Update, x.PersistChange));
        Assert.Single(result.SubEntities.Where(x => x.PersistChange == PersistChange.Insert));
        Assert.Single(result.SubEntities.Where(x => x.PersistChange == PersistChange.Delete));
        // operations are still detected even if not configured
        Assert.Equal(6, operations.Count);
        Assert.Single(operations.OfType<DeleteDiffOperation>());
        Assert.Single(operations.OfType<InsertDiffOperation>());
        Assert.Equal(4, operations.OfType<UpdateDiffOperation>().Count());
    }

    [Theory]
    [InlineData(EqualityComparers.Precompiled)]
    [InlineData(EqualityComparers.Naive)]
    public void NoInsertConfig(EqualityComparers equalityComparer)
    {
        var (existingEntity, newEntity) = GenerateEntities();

        //
        var diffConfiguration = new DeepDiffConfiguration();
        diffConfiguration.ConfigureEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .HasMany(x => x.SubEntities)
            //.OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));
        diffConfiguration.ConfigureEntity<EntityLevel1>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power, x.Price })
            //.OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var listener = new StoreAllOperationListener();
        var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
        var operations = listener.Operations;

        // 1 insert, 1 delete, 4 updates -> insert and delete will be detected
        Assert.NotNull(result);
        Assert.Equal(PersistChange.None, result.PersistChange);
        Assert.All(result.SubEntities, x => Assert.NotEqual(PersistChange.Insert, x.PersistChange));
        Assert.Equal(4, result.SubEntities.Count(x => x.PersistChange == PersistChange.Update));
        Assert.Single(result.SubEntities.Where(x => x.PersistChange == PersistChange.Delete));
        Assert.Empty(result.SubEntities.Where(x => x.PersistChange == PersistChange.Insert));
        // operations are still detected even if not configured
        Assert.Equal(6, operations.Count);
        Assert.Single(operations.OfType<DeleteDiffOperation>());
        Assert.Single(operations.OfType<InsertDiffOperation>());
        Assert.Equal(4, operations.OfType<UpdateDiffOperation>().Count());
    }

    [Theory]
    [InlineData(EqualityComparers.Precompiled)]
    [InlineData(EqualityComparers.Naive)]
    public void NoDeleteConfig(EqualityComparers equalityComparer)
    {
        var (existingEntity, newEntity) = GenerateEntities();

        //
        var diffConfiguration = new DeepDiffConfiguration();
        diffConfiguration.ConfigureEntity<EntityLevel0>()
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .HasMany(x => x.SubEntities)
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update));
            //.OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));
        diffConfiguration.ConfigureEntity<EntityLevel1>()
            .HasKey(x => x.Timestamp)
            .HasValues(x => new { x.Power, x.Price })
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update));
            //.OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));

        var deepDiff = diffConfiguration.CreateDeepDiff();
        var listener = new StoreAllOperationListener();
        var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
        var operations = listener.Operations;

        // 1 insert, 1 delete, 4 updates -> insert and updates will be detected
        Assert.NotNull(result);
        Assert.Equal(PersistChange.None, result.PersistChange);
        Assert.All(result.SubEntities, x => Assert.NotEqual(PersistChange.Delete, x.PersistChange));
        Assert.Equal(4, result.SubEntities.Count(x => x.PersistChange == PersistChange.Update));
        Assert.Single(result.SubEntities.Where(x => x.PersistChange == PersistChange.Insert));
        Assert.Empty(result.SubEntities.Where(x => x.PersistChange == PersistChange.Delete));
        // operations are still detected even if not configured
        Assert.Equal(6, operations.Count);
        Assert.Single(operations.OfType<DeleteDiffOperation>());
        Assert.Single(operations.OfType<InsertDiffOperation>());
        Assert.Equal(4, operations.OfType<UpdateDiffOperation>().Count());
    }

    private (EntityLevel0 existingEntity, EntityLevel0 newEntity) GenerateEntities()
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
        return (existingEntity, newEntity);
    }
}
