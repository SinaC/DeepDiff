using EntityMerger.Configuration;
using EntityMerger.UnitTest.Entities;
using EntityMerger.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace EntityMerger.UnitTest.Simple
{
    public class SimpleEntityMultiLevelNavigationTests
    {
        [Fact]
        public void CheckPropagation()
        {
            var existingEntities = Array.Empty<EntityLevel0>();

            var newEntities = Enumerable.Range(0, 5)
                .Select(x => new EntityLevel0
                {
                    Index = x,

                    Id = Guid.NewGuid(),

                    StartsOn = DateTime.Now,
                    Direction = Direction.Up,

                    RequestedPower = x,
                    Penalty = x,

                    SubEntity = new EntityLevel1
                    {
                        Index = 1000,

                        Id = Guid.NewGuid(),

                        Timestamp = DateTime.Now,

                        Power = x + 1000,
                        Price = x * 1000,

                        SubEntities = Enumerable.Range(0, 5)
                                .Select(z => new EntityLevel2
                                {
                                    Index = z,

                                    Id = Guid.NewGuid(),

                                    DeliveryPointEan = $"DP_{x}_{1000}_{z}",

                                    Value1 = x + 1000 + z,
                                    Value2 = x * 1000 * z,
                                }).ToList()
                    },

                    SubEntities = Enumerable.Range(0, 5)
                        .Select(y => new EntityLevel1
                        {
                            Index = y,

                            Id = Guid.NewGuid(),

                            Timestamp = DateTime.Now,

                            Power = x+y,
                            Price = x*y,

                            SubEntity = new EntityLevel2
                            {
                                Index = 1000,

                                Id = Guid.NewGuid(),

                                DeliveryPointEan = $"DP_{x}_{y}_{1000}",

                                Value1 = x + y + 1000,
                                Value2 = x * y * 1000,
                            },

                            SubEntities = Enumerable.Range(0,5)
                                .Select(z => new EntityLevel2
                                {
                                    Index = z,

                                    Id = Guid.NewGuid(),

                                    DeliveryPointEan = $"DP_{x}_{y}_{z}",

                                    Value1 = x+y+z,
                                    Value2 = x*y*z,
                                }).ToList()
                        }).ToList()
                }).ToList();

            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.PersistEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .HasAdditionalValuesToCopy(x => new { x.AdditionalValueToCopy })
                .HasOne(x => x.SubEntity)
                .HasMany(x => x.SubEntities);
            mergeConfiguration.PersistEntity<EntityLevel1>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.Power, x.Price })
                .HasOne(x => x.SubEntity)
                .HasMany(x => x.SubEntities);
            mergeConfiguration.PersistEntity<EntityLevel2>()
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.Value1, x.Value2});

            var merger = mergeConfiguration.CreateMerger();
            var results = merger.Merge(existingEntities, newEntities).ToArray();

            Assert.Equal(5, results.Length);
            Assert.Equal(25, results.SelectMany(x => x.SubEntities).Count());
            Assert.Equal(125, results.SelectMany(x => x.SubEntities).SelectMany(x => x.SubEntities).Count());
            Assert.All(results, x => Assert.Equal(PersistChange.Insert, x.PersistChange));
            Assert.All(results.SelectMany(x => x.SubEntities), x => Assert.Equal(PersistChange.Insert, x.PersistChange));
            Assert.All(results.SelectMany(x => x.SubEntities).SelectMany(x => x.SubEntities), x => Assert.Equal(PersistChange.Insert, x.PersistChange));
            Assert.All(results.Select(x => x.SubEntity), x => Assert.Equal(PersistChange.Insert, x.PersistChange));
            Assert.All(results.SelectMany(x => x.SubEntities).Select(x => x.SubEntity), x => Assert.Equal(PersistChange.Insert, x.PersistChange));
        }
    }
}
