using EntityMerger.Configuration;
using EntityMerger.UnitTest.Entities;
using EntityMerger.UnitTest.Entities.Simple;
using System;
using System.Collections.Generic;
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

            var newEntities = GenerateEntities(DateTime.Now).ToList();

            var merger = CreateMerger();
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

        [Fact]
        public void UpdateSetWhenInsertingNavigationManyNestedEntity()
        {
            var now = DateTime.Now;
            // existing == newEntities
            var existingEntities = GenerateEntities(now).ToList();
            var newEntities = GenerateEntities(now).ToList();
            // add new sub-sub-entity
            newEntities[3].SubEntities[2].SubEntities.Add(new EntityLevel2
            {
                Index = 98765,

                Id = Guid.NewGuid(),

                DeliveryPointEan = $"DP_INSERTED",

                Value1 = 1234,
                Value2 = 5678,
            });

            var merger = CreateMerger();
            var results = merger.Merge(existingEntities, newEntities).ToArray();

            // Entity0: 4th -> updated
            Assert.Single(results);
            Assert.Equal(PersistChange.Update, results.Single().PersistChange);
            Assert.Equal(3, results.Single().Index);
            // Entity1: 3rd -> updated
            Assert.Single(results.Single().SubEntities);
            Assert.Equal(PersistChange.Update, results.Single().SubEntities.Single().PersistChange);
            Assert.Equal(3 * 10 + 2, results.Single().SubEntities.Single().Index);
            // Entity2: 6th -> inserted
            Assert.Single(results.Single().SubEntities.Single().SubEntities);
            Assert.Equal(PersistChange.Insert, results.Single().SubEntities.Single().SubEntities.Single().PersistChange);
            Assert.Equal("DP_INSERTED", results.Single().SubEntities.Single().SubEntities.Single().DeliveryPointEan);
            Assert.Equal(98765, results.Single().SubEntities.Single().SubEntities.Single().Index);
        }

        [Fact]
        public void UpdateSetWhenDeletingNavigationManyNestedEntity()
        {
            var now = DateTime.Now;
            // existing == newEntities
            var existingEntities = GenerateEntities(now).ToList();
            var newEntities = GenerateEntities(now).ToList();
            // delete 2nd sub-sub-entity
            newEntities[3].SubEntities[2].SubEntities.RemoveAt(1);

            var merger = CreateMerger();
            var results = merger.Merge(existingEntities, newEntities).ToArray();

            // Entity0: 4th -> updated
            Assert.Single(results);
            Assert.Equal(PersistChange.Update, results.Single().PersistChange);
            Assert.Equal(3, results.Single().Index);
            // Entity1: 3rd -> updated
            Assert.Single(results.Single().SubEntities);
            Assert.Equal(PersistChange.Update, results.Single().SubEntities.Single().PersistChange);
            Assert.Equal(3 * 10 + 2, results.Single().SubEntities.Single().Index);
            // Entity2: 2th -> deleted
            Assert.Single(results.Single().SubEntities.Single().SubEntities);
            Assert.Equal(PersistChange.Delete, results.Single().SubEntities.Single().SubEntities.Single().PersistChange);
            Assert.Equal("DP_3_2_1", results.Single().SubEntities.Single().SubEntities.Single().DeliveryPointEan);
            Assert.Equal(3 * 100 + 2 * 10 + 1, results.Single().SubEntities.Single().SubEntities.Single().Index);
        }

        [Fact]
        public void UpdateSetWhenUpdatingNavigationManyNestedEntity()
        {
            var now = DateTime.Now;
            // existing == newEntities
            var existingEntities = GenerateEntities(now).ToList();
            var newEntities = GenerateEntities(now).ToList();
            // update 2nd sub-sub-entity
            newEntities[3].SubEntities[2].SubEntities[1].Value2 = 13579m;

            var merger = CreateMerger();
            var results = merger.Merge(existingEntities, newEntities).ToArray();

            // Entity0: 4th -> updated
            Assert.Single(results);
            Assert.Equal(PersistChange.Update, results.Single().PersistChange);
            Assert.Equal(3, results.Single().Index);
            // Entity1: 3rd -> updated
            Assert.Single(results.Single().SubEntities);
            Assert.Equal(PersistChange.Update, results.Single().SubEntities.Single().PersistChange);
            Assert.Equal(3 * 10 + 2, results.Single().SubEntities.Single().Index);
            // Entity2: 2th -> updated
            Assert.Single(results.Single().SubEntities.Single().SubEntities);
            Assert.Equal(PersistChange.Update, results.Single().SubEntities.Single().SubEntities.Single().PersistChange);
            Assert.Equal(3 * 100 + 2 * 10 + 1, results.Single().SubEntities.Single().SubEntities.Single().Index);
            Assert.Equal("DP_3_2_1", results.Single().SubEntities.Single().SubEntities.Single().DeliveryPointEan);
            Assert.Equal(13579m, results.Single().SubEntities.Single().SubEntities.Single().Value2);
        }

        [Fact]
        public void UpdateSetAndDeletePropagationWhenDeletingNavigationManyNestedEntity()
        {
            var now = DateTime.Now;
            // existing == newEntities
            var existingEntities = GenerateEntities(now).ToList();
            var newEntities = GenerateEntities(now).ToList();
            // delete 3rd sub-entity
            newEntities[3].SubEntities.RemoveAt(2);

            var merger = CreateMerger();
            var results = merger.Merge(existingEntities, newEntities).ToArray();

            // Entity0: 4th -> updated
            Assert.Single(results);
            Assert.Equal(PersistChange.Update, results.Single().PersistChange);
            Assert.Equal(3, results.Single().Index);
            // Entity1: 3rd -> deleted
            Assert.Single(results.Single().SubEntities);
            Assert.Equal(PersistChange.Delete, results.Single().SubEntities.Single().PersistChange);
            Assert.Equal(3 * 10 + 2, results.Single().SubEntities.Single().Index);
            // Every Entity2 in Entity1 -> deleted
            Assert.Equal(5, results.Single().SubEntities.Single().SubEntities.Count);
            Assert.All(results.Single().SubEntities.Single().SubEntities, x => Assert.Equal(PersistChange.Delete, x.PersistChange));
        }

        [Fact]
        public void UpdateSetWhenUpdatingNavigationOneNestedEntity()
        {
            var now = DateTime.Now;
            // existing == newEntities
            var existingEntities = GenerateEntities(now).ToList();
            var newEntities = GenerateEntities(now).ToList();
            // update sub-sub-entity
            newEntities[3].SubEntities[2].SubEntity.Value2 = 13579m;

            var merger = CreateMerger();
            var results = merger.Merge(existingEntities, newEntities).ToArray();

            // Entity0: 4th -> updated
            Assert.Single(results);
            Assert.Equal(PersistChange.Update, results.Single().PersistChange);
            Assert.Equal(3, results.Single().Index);
            // Entity1: 3rd -> updated
            Assert.Single(results.Single().SubEntities);
            Assert.Equal(PersistChange.Update, results.Single().SubEntities.Single().PersistChange);
            Assert.Equal(3 * 10 + 2, results.Single().SubEntities.Single().Index);
            // Entity2: update
            Assert.Equal(PersistChange.Update, results.Single().SubEntities.Single().SubEntity.PersistChange);
            Assert.Equal(3 * 10 + 2 + 9999, results.Single().SubEntities.Single().SubEntity.Index);
            Assert.Equal(13579m, results.Single().SubEntities.Single().SubEntity.Value2);
            Assert.Equal("DP_3_2_1000", results.Single().SubEntities.Single().SubEntity.DeliveryPointEan);
        }

        [Fact]
        public void UpdateSetWhenInsertingNavigationManyInsideNavigationOneNestedEntity()
        {
            var now = DateTime.Now;
            // existing == newEntities
            var existingEntities = GenerateEntities(now).ToList();
            var newEntities = GenerateEntities(now).ToList();
            // add new sub-sub-entity
            newEntities[3].SubEntity.SubEntities.Add(new EntityLevel2
            {
                Index = 98765,

                Id = Guid.NewGuid(),

                DeliveryPointEan = $"DP_INSERTED",

                Value1 = 1234,
                Value2 = 5678,
            });

            var merger = CreateMerger();
            var results = merger.Merge(existingEntities, newEntities).ToArray();

            // Entity0: 4th -> updated
            Assert.Single(results);
            Assert.Equal(PersistChange.Update, results.Single().PersistChange);
            Assert.Equal(3, results.Single().Index);
            // Entity1: updated
            Assert.Equal(PersistChange.Update, results.Single().SubEntity.PersistChange);
            Assert.Equal(3 + 9999, results.Single().SubEntity.Index);
            // Entity2: 6th -> inserted
            Assert.Single(results.Single().SubEntity.SubEntities);
            Assert.Equal(PersistChange.Insert, results.Single().SubEntity.SubEntities.Single().PersistChange);
            Assert.Equal(98765, results.Single().SubEntity.SubEntities.Single().Index);
            Assert.Equal("DP_INSERTED", results.Single().SubEntity.SubEntities.Single().DeliveryPointEan);
        }

        [Fact]
        public void UpdateSetWhenDeletingNavigationManyInsideNavigationOneNestedEntity()
        {
            var now = DateTime.Now;
            // existing == newEntities
            var existingEntities = GenerateEntities(now).ToList();
            var newEntities = GenerateEntities(now).ToList();
            // delete 2nd sub-sub-entity
            newEntities[3].SubEntity.SubEntities.RemoveAt(1);

            var merger = CreateMerger();
            var results = merger.Merge(existingEntities, newEntities).ToArray();

            // Entity0: 4th -> updated
            Assert.Single(results);
            Assert.Equal(PersistChange.Update, results.Single().PersistChange);
            Assert.Equal(3, results.Single().Index);
            // Entity1: updated
            Assert.Equal(PersistChange.Update, results.Single().SubEntity.PersistChange);
            Assert.Equal(3 + 9999, results.Single().SubEntity.Index);
            // Entity2: 2th -> deleted
            Assert.Single(results.Single().SubEntity.SubEntities);
            Assert.Equal(PersistChange.Delete, results.Single().SubEntity.SubEntities.Single().PersistChange);
            Assert.Equal(3 * 10 + 1 + 9999, results.Single().SubEntity.SubEntities.Single().Index);
            Assert.Equal("DP_3_1000_1", results.Single().SubEntity.SubEntities.Single().DeliveryPointEan);
        }

        [Fact]
        public void UpdateSetWhenUpdatingNavigationManyInsideNavigationOneNestedEntity()
        {
            var now = DateTime.Now;
            // existing == newEntities
            var existingEntities = GenerateEntities(now).ToList();
            var newEntities = GenerateEntities(now).ToList();
            // update 2nd sub-sub-entity
            newEntities[3].SubEntity.SubEntities[1].Value2 = 13579m;

            var merger = CreateMerger();
            var results = merger.Merge(existingEntities, newEntities).ToArray();

            // Entity0: 4th -> updated
            Assert.Single(results);
            Assert.Equal(PersistChange.Update, results.Single().PersistChange);
            Assert.Equal(3, results.Single().Index);
            // Entity1: 3rd -> updated
            Assert.Equal(PersistChange.Update, results.Single().SubEntity.PersistChange);
            Assert.Equal(3 + 9999, results.Single().SubEntity.Index);
            // Entity2: 2th -> updated
            Assert.Single(results.Single().SubEntity.SubEntities);
            Assert.Equal(PersistChange.Update, results.Single().SubEntity.SubEntities.Single().PersistChange);
            Assert.Equal(3 * 10 + 1 + 9999, results.Single().SubEntity.SubEntities.Single().Index);
            Assert.Equal("DP_3_1000_1", results.Single().SubEntity.SubEntities.Single().DeliveryPointEan);
            Assert.Equal(13579m, results.Single().SubEntity.SubEntities.Single().Value2);
        }

        private static IMerger CreateMerger()
        {
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
                .HasValues(x => new { x.Value1, x.Value2 });
            var merger = mergeConfiguration.CreateMerger();
            return merger;
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

                    SubEntities = Enumerable.Range(0, 5)
                        .Select(y => new EntityLevel1
                        {
                            Index = x * 10 + y,

                            Id = Guid.NewGuid(),

                            Timestamp = (now ?? DateTime.Now).AddHours(x).AddMinutes(y),

                            Power = x + y,
                            Price = x * y,

                            SubEntity = new EntityLevel2
                            {
                                Index = x * 10 + y + 9999,

                                Id = Guid.NewGuid(),

                                DeliveryPointEan = $"DP_{x}_{y}_{1000}",

                                Value1 = x + y + 1000,
                                Value2 = x * y * 1000,
                            },

                            SubEntities = Enumerable.Range(0, 5)
                                .Select(z => new EntityLevel2
                                {
                                    Index = x * 100 + y * 10 + z,

                                    Id = Guid.NewGuid(),

                                    DeliveryPointEan = $"DP_{x}_{y}_{z}",

                                    Value1 = x + y + z,
                                    Value2 = x * y * z,
                                }).ToList()
                        }).ToList()
                }).ToList();
        }
    }
}
