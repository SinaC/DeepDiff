﻿using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Simple;

// EntityLevel0
//      0/1 EntityLevel1 NoKey
//          0/Many EntityLevel2
public class NoValuesTests
{
    [Theory]
    [InlineData(EqualityComparers.Precompiled)]

    [InlineData(EqualityComparers.Naive)]

    public void Identical(EqualityComparers equalityComparer)
    {

        var now = DateTime.Now;
        // existing == newEntities
        var existingEntities = GenerateEntities(now).ToList();
        var newEntities = GenerateEntities(now).ToList();

        //
        var deepDiff = CreateDeepDiff();
        var entities = deepDiff.MergeMany(existingEntities, newEntities, opt => opt.SetEqualityComparer(equalityComparer));

        //
        Assert.Empty(entities);
    }

    [Theory]
    [InlineData(EqualityComparers.Precompiled)]
    [InlineData(EqualityComparers.Naive)]
    public void Level1_Update(EqualityComparers equalityComparer)
    {

        var now = DateTime.Now;
        // existing == newEntities
        var existingEntities = GenerateEntities(now).ToList();
        var newEntities = GenerateEntities(now).ToList();
        newEntities[2].SubEntity.Power = -500m;

        //
        var deepDiff = CreateDeepDiff();
        var entities = deepDiff.MergeMany(existingEntities, newEntities, opt => opt.SetEqualityComparer(equalityComparer));

        //
        Assert.Single(entities);
        Assert.Equal(2, entities.Single().Index);
        Assert.Equal(PersistChange.None, entities.Single().PersistChange);
        Assert.Empty(entities.Single().SubEntity.SubEntities);
        Assert.Equal(PersistChange.Update, entities.Single().SubEntity.PersistChange);
        Assert.Equal(-500m, entities.Single().SubEntity.Power);
    }

    [Theory]
    [InlineData(EqualityComparers.Precompiled)]
    [InlineData(EqualityComparers.Naive)]
    public void Level2_Update(EqualityComparers equalityComparer)
    {

        var now = DateTime.Now;
        // existing == newEntities
        var existingEntities = GenerateEntities(now).ToList();
        var newEntities = GenerateEntities(now).ToList();
        newEntities[2].SubEntity.SubEntities[3].Value1 = -500m;

        //
        var deepDiff = CreateDeepDiff();
        var results = deepDiff.MergeMany(existingEntities, newEntities, opt => opt.SetEqualityComparer(equalityComparer));

        //
        Assert.Empty(results); // no modification detected because only values on entity level 2 has changed and entity level 2 has no declared HasValues in config
    }

    private static IDeepDiff CreateDeepDiff()
    {
        var diffConfiguration = new DeepDiffConfiguration();
        diffConfiguration.ConfigureEntity<EntityLevel0>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => new { x.StartsOn, x.Direction })
            .HasValues(x => new { x.RequestedPower, x.Penalty })
            .OnUpdate(cfg => cfg.CopyValues(x => x.AdditionalValueToCopy))
            .HasOne(x => x.SubEntity);
        diffConfiguration.ConfigureEntity<EntityLevel1>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .NoKey()
            .HasValues(x => new { x.Power, x.Price })
            .HasMany(x => x.SubEntities);
        diffConfiguration.ConfigureEntity<EntityLevel2>()
            .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
            .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
            .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
            .HasKey(x => x.DeliveryPointEan);
            //don't declare HasValues to be able to test a regression .HasValues(x => new { x.Value1, x.Value2 });
        var deepDiff = diffConfiguration.CreateDeepDiff();
        return deepDiff;
    }

    private static IEnumerable<EntityLevel0> GenerateEntities(DateTime? now)
    {
        return Enumerable.Range(0, 5)
            .Select(x => new EntityLevel0
            {
                Index = x,

                Id = Guid.NewGuid(),

                StartsOn = (now ?? DateTime.Now).AddHours(x),
                Direction = Direction.Up,

                RequestedPower = x,
                Penalty = x,

                SubEntity = new EntityLevel1
                {
                    Index = x + 9999,

                    Id = Guid.NewGuid(),

                    Timestamp = now ?? DateTime.Now,

                    Power = x + 1000,
                    Price = x * 1000,

                    SubEntities = Enumerable.Range(0, 5)
                            .Select(z => new EntityLevel2
                            {
                                Index = x * 10 + z + 9999,

                                Id = Guid.NewGuid(),

                                DeliveryPointEan = $"DP_{x}_{1000}_{z}",

                                Value1 = x + 1000 + z,
                                Value2 = x * 1000 * z,
                            }).ToList()
                },
            }).ToList();
    }
}
