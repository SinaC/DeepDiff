using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.OperationListener
{
    public class MergeTests
    {
        [Theory]
        [InlineData(EqualityComparers.Precompiled, true)]
        [InlineData(EqualityComparers.Precompiled, false)]
        [InlineData(EqualityComparers.Naive, true)]
        [InlineData(EqualityComparers.Naive, false)]
        public void MergeSingle(EqualityComparers equalityComparer, bool defineOperationsOnEntity)
        {
            var (existingEntity, newEntity) = GenerateEntities(0);

            var deepDiff = CreateDeepDiff(defineOperationsOnEntity);
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            Assert.NotEmpty(operations);
            // 1 delete
            Assert.Single(operations.OfType<DeleteDiffOperation>());
            Assert.Single(operations.OfType<DeleteDiffOperation>(), x => x.EntityName == nameof(EntityLevel1));
            Assert.Single(operations.OfType<DeleteDiffOperation>().Single().Keys);
            Assert.Equal(nameof(EntityLevel1.Timestamp), operations.OfType<DeleteDiffOperation>().Single().Keys.Single().Key);
            Assert.Equal(DateTime.Today.ToString(), operations.OfType<DeleteDiffOperation>().Single().Keys.Single().Value);
            // 1 insert
            Assert.Single(operations.OfType<InsertDiffOperation>());
            Assert.Single(operations.OfType<InsertDiffOperation>(), x => x.EntityName == nameof(EntityLevel1));
            Assert.Single(operations.OfType<InsertDiffOperation>().Single().Keys);
            Assert.Equal(nameof(EntityLevel1.Timestamp), operations.OfType<InsertDiffOperation>().Single().Keys.Single().Key);
            Assert.Equal(DateTime.Today.AddMinutes(5).ToString(), operations.OfType<InsertDiffOperation>().Single().Keys.Single().Value);
            // 1 update on EntityLevel0 and 4 updates on EntityLevel1
            Assert.Equal(5, operations.OfType<UpdateDiffOperation>().Count());
            Assert.Equal(4, operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).Count());
            Assert.Single(operations.OfType<UpdateDiffOperation>(), x => x.EntityName == nameof(EntityLevel0));
            Assert.Equal(4, operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.Keys).Count());
            Assert.Equal(2, operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)).SelectMany(x => x.Keys).Count());
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)).SelectMany(x => x.Keys), x => x.Key == nameof(EntityLevel0.StartsOn));
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)).SelectMany(x => x.Keys), x => x.Key == nameof(EntityLevel0.Direction));
            Assert.Equal(DateTime.Today.ToString(), operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)).SelectMany(x => x.Keys).Single(x => x.Key == nameof(EntityLevel0.StartsOn)).Value);
            Assert.Equal(Direction.Up.ToString(), operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)).SelectMany(x => x.Keys).Single(x => x.Key == nameof(EntityLevel0.Direction)).Value);
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.Keys).Select(x => x.Key), x => Assert.Equal(nameof(EntityLevel1.Timestamp), x));
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.Keys), x => x.Value == DateTime.Today.AddMinutes(1).ToString());
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.Keys), x => x.Value == DateTime.Today.AddMinutes(2).ToString());
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.Keys), x => x.Value == DateTime.Today.AddMinutes(3).ToString());
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.Keys), x => x.Value == DateTime.Today.AddMinutes(4).ToString());
            // each update EntityLevel1 are on Power property -> generate 1 UpdatedProperty (Power)
            //
            Assert.Single(operations.OfType<UpdateDiffOperation>(), x => x.EntityName == nameof(EntityLevel0));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)), x => Assert.Single(x.UpdatedProperties));
            Assert.Equal(nameof(EntityLevel0.Penalty), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(EntityLevel0)).UpdatedProperties.Single().PropertyName);
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)), x => Assert.Empty(x.NavigationParentKeys)); // no parent
            //
            Assert.Equal(4, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(EntityLevel1)));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.Single(x.UpdatedProperties));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.Equal(nameof(EntityLevel1.Power), x.PropertyName));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.NotEqual(x.ExistingValue, x.NewValue));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.Equal(Convert.ToInt32(x.ExistingValue) * 2, Convert.ToInt32(x.NewValue)));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.Single(x.NavigationParentKeys)); // one parent
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.All(x.NavigationParentKeys, y => Assert.Equal(nameof(EntityLevel0), y.Key))); // one parent of EnityLevel0
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled, true)]
        [InlineData(EqualityComparers.Precompiled, false)]
        [InlineData(EqualityComparers.Naive, true)]
        [InlineData(EqualityComparers.Naive, false)]
        public void CheckNavigationPathKeys(EqualityComparers equalityComparer, bool defineOperationsOnEntity)
        {
            var existingEntity = new EntityLevel0
            {
                Id = Guid.NewGuid(),
                StartsOn = DateTime.Today,
                Direction = Direction.Up,
                RequestedPower = 1,
                Penalty = 3,
                Comment = $"Existing",
                SubEntities = new List<EntityLevel1>
                {
                    new EntityLevel1
                    {
                        Id = Guid.NewGuid(),
                        Timestamp = DateTime.Today,
                        Power = 1,
                        Price = 2,
                        Comment = $"Existing_1",

                        SubEntities = new List<EntityLevel2>
                        {
                            new EntityLevel2
                            {
                                Id = Guid.NewGuid(),
                                DeliveryPointEan = "EAN1",
                                Value1 = 1,
                                Value2 = 2,
                                Value3 = 3,
                                Value4 = 4,
                            }
                        }
                    }
                }
            };
            var newEntity = new EntityLevel0 // identical to existing but entity level 2 has one modified value
            {
                Id = Guid.NewGuid(),
                StartsOn = DateTime.Today,
                Direction = Direction.Up,
                RequestedPower = 1,
                Penalty = 3,
                Comment = $"Existing",
                SubEntities = new List<EntityLevel1>
                {
                    new EntityLevel1
                    {
                        Id = Guid.NewGuid(),
                        Timestamp = DateTime.Today,
                        Power = 1,
                        Price = 2,
                        Comment = $"Existing_1",

                        SubEntities = new List<EntityLevel2>
                        {
                            new EntityLevel2
                            {
                                Id = Guid.NewGuid(),
                                DeliveryPointEan = "EAN1",
                                Value1 = 1,
                                Value2 = 3,
                            }
                        }
                    }
                }
            };

            var diffConfiguration = new DeepDiffConfiguration();
            if (defineOperationsOnEntity)
            {
                diffConfiguration.ConfigureEntity<EntityLevel0>()
                    .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                    .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                    .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                    .HasKey(x => new { x.StartsOn, x.Direction })
                    .HasValues(x => new { x.RequestedPower, x.Penalty })
                    .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
                    .HasMany(x => x.SubEntities)
                    .Ignore(x => x.Index);
                diffConfiguration.ConfigureEntity<EntityLevel1>()
                    .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                    .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                    .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                    .HasKey(x => x.Timestamp)
                    .HasValues(x => new { x.Power, x.Price })
                    .HasMany(x => x.SubEntities)
                    .Ignore(x => x.Index);
                diffConfiguration.ConfigureEntity<EntityLevel2>()
                    .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                    .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                    .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                    .HasKey(x => x.DeliveryPointEan)
                    .HasValues(x => new { x.Value1, x.Value2 })
                    .Ignore(x => x.Index);
            }
            else
            {
                diffConfiguration.ConfigureEntity<EntityLevel0>()
                    .HasKey(x => new { x.StartsOn, x.Direction })
                    .HasValues(x => new { x.RequestedPower, x.Penalty })
                    .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
                    .HasMany(x => x.SubEntities)
                    .Ignore(x => x.Index)
                    .Ignore(x => x.PersistChange);
                diffConfiguration.ConfigureEntity<EntityLevel1>()
                    .HasKey(x => x.Timestamp)
                    .HasValues(x => new { x.Power, x.Price })
                    .HasMany(x => x.SubEntities)
                    .Ignore(x => x.Index)
                    .Ignore(x => x.PersistChange);
                diffConfiguration.ConfigureEntity<EntityLevel2>()
                    .HasKey(x => x.DeliveryPointEan)
                    .HasValues(x => new { x.Value1, x.Value2 })
                    .Ignore(x => x.Index)
                    .Ignore(x => x.PersistChange);

            }

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var listener = new StoreAllOperationListener();
            var result = deepDiff.MergeSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            // only check entity2 update operations
            Assert.Single(operations.OfType<UpdateDiffOperation>(), x => x.EntityName == nameof(EntityLevel2));
            Assert.Equal(2, operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(EntityLevel2)).NavigationParentKeys.Count); // parent and grand-parent
            Assert.Single(operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(EntityLevel2)).NavigationParentKeys, x => x.Key == nameof(EntityLevel1)); // one parent
            Assert.Single(operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(EntityLevel2)).NavigationParentKeys, x => x.Key == nameof(EntityLevel0)); // one grand-parent
        }

        private static (EntityLevel0 existingEntity, EntityLevel0 newEntity) GenerateEntities(int level0Index)
        {
            var existingEntity = GenerateExisting(level0Index);
            var newEntity = GenerateNew(level0Index);
            return (existingEntity, newEntity);
        }

        private static EntityLevel0 GenerateExisting(int level0Index)
            => new()
            {
                Index = level0Index,

                Id = Guid.NewGuid(),
                StartsOn = DateTime.Today.AddDays(level0Index),
                Direction = Direction.Up,
                RequestedPower = 1,
                Penalty = 3,
                Comment = $"Existing",
                AdditionalValueToCopy = $"ExistingAdditionalValue",
                SubEntities = Enumerable.Range(0, 5).Select(y => new EntityLevel1
                {
                    Index = level0Index * 1000 + y,

                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Today.AddDays(level0Index).AddMinutes(y),
                    Power = y,
                    Price = y % 2 == 0 ? null : y * 3,
                    Comment = $"Existing_{y}",
                }).ToList(),
            };

        private static EntityLevel0 GenerateNew(int level0Index)
            => new()
            {
                Index = level0Index,

                Id = Guid.NewGuid(),
                StartsOn = DateTime.Today.AddDays(level0Index),
                Direction = Direction.Up,
                RequestedPower = 1,
                Penalty = 4,
                Comment = $"New",
                AdditionalValueToCopy = $"NewAdditionalValue",
                SubEntities = Enumerable.Range(1, 5).Select(y => new EntityLevel1
                {
                    Index = level0Index * 1000 + y,

                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Today.AddDays(level0Index).AddMinutes(y),
                    Power = 2 * y,
                    Price = y % 2 == 0 ? null : y * 3,
                    Comment = $"New_{y}",
                }).ToList(),
            };

        private static IDeepDiff CreateDeepDiff(bool defineOperationsOnEntity)
        {
            var diffConfiguration = new DeepDiffConfiguration();
            if (defineOperationsOnEntity)
            {
                diffConfiguration.ConfigureEntity<EntityLevel0>()
                    .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                    .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                    .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                    .HasKey(x => new { x.StartsOn, x.Direction })
                    .HasValues(x => new { x.RequestedPower, x.Penalty })
                    .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
                    .HasMany(x => x.SubEntities)
                    .Ignore(x => x.Index);
                diffConfiguration.ConfigureEntity<EntityLevel1>()
                    .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                    .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                    .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                    .HasKey(x => x.Timestamp)
                    .HasValues(x => new { x.Power, x.Price })
                    .Ignore(x => x.Index);
            }
            else
            {
                diffConfiguration.ConfigureEntity<EntityLevel0>()
                    .HasKey(x => new { x.StartsOn, x.Direction })
                    .HasValues(x => new { x.RequestedPower, x.Penalty })
                    .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
                    .HasMany(x => x.SubEntities)
                    .Ignore(x => x.Index)
                    .Ignore(x => x.PersistChange);
                diffConfiguration.ConfigureEntity<EntityLevel1>()
                    .HasKey(x => x.Timestamp)
                    .HasValues(x => new { x.Power, x.Price })
                    .Ignore(x => x.Index)
                    .Ignore(x => x.PersistChange);
            }

            return diffConfiguration.CreateDeepDiff();
        }
    }
}
