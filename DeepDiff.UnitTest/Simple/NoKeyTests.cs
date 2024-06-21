using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System.Collections.Generic;
using System.Linq;
using System;
using Xunit;

namespace DeepDiff.UnitTest.Simple
{
    // EntityLevel0
    //      0/1 EntityLevel1 NoKey
    //          0/Many EntityLevel2
    public class NoKeyTests
    {
        [Fact]
        public void Identical()
        {

            var now = DateTime.Now;
            // existing == newEntities
            var existingEntities = GenerateEntities(now).ToList();
            var newEntities = GenerateEntities(now).ToList();

            //
            var deepDiff = CreateDeepDiff();
            var diff = deepDiff.MergeMany(existingEntities, newEntities);

            //
            Assert.Empty(diff.Entities);
        }

        [Fact]
        public void Level1_Update()
        {

            var now = DateTime.Now;
            // existing == newEntities
            var existingEntities = GenerateEntities(now).ToList();
            var newEntities = GenerateEntities(now).ToList();
            newEntities[2].SubEntity.Power = -500m;

            //
            var deepDiff = CreateDeepDiff();
            var diff = deepDiff.MergeMany(existingEntities, newEntities);

            //
            Assert.Single(diff.Entities);
            Assert.Equal(2, diff.Entities.Single().Index);
            Assert.Equal(PersistChange.None, diff.Entities.Single().PersistChange);
            Assert.Empty(diff.Entities.Single().SubEntity.SubEntities);
            Assert.Equal(PersistChange.Update, diff.Entities.Single().SubEntity.PersistChange);
            Assert.Equal(-500m, diff.Entities.Single().SubEntity.Power);
        }

        [Fact]
        public void Level2_Update()
        {

            var now = DateTime.Now;
            // existing == newEntities
            var existingEntities = GenerateEntities(now).ToList();
            var newEntities = GenerateEntities(now).ToList();
            newEntities[2].SubEntity.SubEntities[3].Value1 = -500m;

            //
            var deepDiff = CreateDeepDiff();
            var diff = deepDiff.MergeMany(existingEntities, newEntities);

            //
            Assert.Single(diff.Entities);
            Assert.Equal(2, diff.Entities.Single().Index);
            Assert.Equal(PersistChange.None, diff.Entities.Single().PersistChange);
            Assert.Single(diff.Entities.Single().SubEntity.SubEntities);
            Assert.Equal(PersistChange.None, diff.Entities.Single().SubEntity.PersistChange);
            Assert.Equal(2 * 10 + 3 + 9999, diff.Entities.Single().SubEntity.SubEntities.Single().Index);
            Assert.Equal(-500m, diff.Entities.Single().SubEntity.SubEntities.Single().Value1);
            Assert.Equal(PersistChange.Update, diff.Entities.Single().SubEntity.SubEntities.Single().PersistChange);
        }

        private IDeepDiff CreateDeepDiff()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => x.AdditionalValueToCopy))
                .HasOne(x => x.SubEntity);
            diffConfiguration.Entity<EntityLevel1>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .NoKey()
                .HasValues(x => new { x.Power, x.Price })
                .HasMany(x => x.SubEntities);
            diffConfiguration.Entity<EntityLevel2>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.Value1, x.Value2 });
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
}
