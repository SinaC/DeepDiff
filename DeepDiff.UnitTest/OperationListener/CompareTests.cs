using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

namespace DeepDiff.UnitTest.OperationListener
{
    public class CompareTests
    {
        [Theory]
        [InlineData(EqualityComparers.Precompiled, true)]
        [InlineData(EqualityComparers.Precompiled, false)]
        [InlineData(EqualityComparers.Naive, true)]
        [InlineData(EqualityComparers.Naive, false)]
        public void CompareSingle(EqualityComparers equalityComparer, bool defineOperationsOnEntity)
        {
            var (existingEntity, newEntity) = GenerateModifications(1);

            var deepDiff = CreateDeepDiff(defineOperationsOnEntity);
            var listener = new StoreAllOperationListener();
            deepDiff.CompareSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            Assert.NotEmpty(operations);
            // 1 delete
            Assert.Single(operations.OfType<DeleteDiffOperation>());
            Assert.Single(operations.OfType<DeleteDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)));
            Assert.NotEmpty(operations.OfType<DeleteDiffOperation>().Single().Keys);
            Assert.All(operations.OfType<DeleteDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)), x => Assert.Empty(x.NavigationParentKeys)); // no parent
            // 1 insert
            Assert.Single(operations.OfType<InsertDiffOperation>());
            Assert.Single(operations.OfType<InsertDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)));
            Assert.NotEmpty(operations.OfType<InsertDiffOperation>().Single().Keys);
            Assert.All(operations.OfType<InsertDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)), x => Assert.Empty(x.NavigationParentKeys)); // no parent
            // 1 update on EntityLevel0 and 4 updates on EntityLevel1
            // each update EntityLevel1 are on Power property -> generate 1 UpdatedProperty (Power)
            Assert.Equal(5, operations.OfType<UpdateDiffOperation>().Count());
            //
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)));
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
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.All(x.NavigationParentKeys, y => Assert.Equal(2, y.Value.Count))); // EntityLevel0 has 2 keys (StartsOn and Direction)
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled, true)]
        [InlineData(EqualityComparers.Precompiled, false)]
        [InlineData(EqualityComparers.Naive, true)]
        [InlineData(EqualityComparers.Naive, false)]
        public void CompareSingle_NoExisting(EqualityComparers equalityComparer, bool defineOperationsOnEntity)
        {
            var existingEntity = (EntityLevel0)null!;
            var newEntity = GenerateNew(1);

            var deepDiff = CreateDeepDiff(defineOperationsOnEntity);
            var listener = new StoreAllOperationListener();
            deepDiff.CompareSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            Assert.NotEmpty(operations);
            // no update/delete
            Assert.Empty(operations.OfType<UpdateDiffOperation>());
            Assert.Empty(operations.OfType<DeleteDiffOperation>());
            // 6 inserts: 1 for EntityLevel0 and 5 for EntityLevel1
            Assert.Equal(6, operations.OfType<InsertDiffOperation>().Count());
            // EntityLevel0
            Assert.Single(operations.OfType<InsertDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)));
            Assert.NotEmpty(operations.OfType<InsertDiffOperation>().Single(x => x.EntityName == nameof(EntityLevel0)).Keys);
            Assert.Empty(operations.OfType<InsertDiffOperation>().Single(x => x.EntityName == nameof(EntityLevel0)).NavigationParentKeys); // no parent
            // EntityLevel1
            Assert.Equal(5, operations.OfType<InsertDiffOperation>().Count(x => x.EntityName == nameof(EntityLevel1)));
            Assert.All(operations.OfType<InsertDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.NotEmpty(x.Keys));
            Assert.All(operations.OfType<InsertDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.NotEmpty(x.NavigationParentKeys));
            Assert.All(operations.OfType<InsertDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.Single(x.NavigationParentKeys.Keys));
            Assert.All(operations.OfType<InsertDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.All(x.NavigationParentKeys, y => Assert.Equal(nameof(EntityLevel0), y.Key))); // one parent of EnityLevel0
            Assert.All(operations.OfType<InsertDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.All(x.NavigationParentKeys, y => Assert.Equal(2, y.Value.Count))); // EntityLevel0 has 2 keys (StartsOn and Direction)
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled, true)]
        [InlineData(EqualityComparers.Precompiled, false)]
        [InlineData(EqualityComparers.Naive, true)]
        [InlineData(EqualityComparers.Naive, false)]
        public void CompareSingle_NoNew(EqualityComparers equalityComparer, bool defineOperationsOnEntity)
        {
            var existingEntity = GenerateExisting(1);
            var newEntity = (EntityLevel0)null!;

            var deepDiff = CreateDeepDiff(defineOperationsOnEntity);
            var listener = new StoreAllOperationListener();
            deepDiff.CompareSingle(existingEntity, newEntity, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            Assert.NotEmpty(operations);
            // no update/insert
            Assert.Empty(operations.OfType<UpdateDiffOperation>());
            Assert.Empty(operations.OfType<InsertDiffOperation>());
            // 6 delete: 1 for EntityLevel0 and 5 for EntityLevel1
            Assert.Equal(6, operations.OfType<DeleteDiffOperation>().Count());
            // EntityLevel0
            Assert.Single(operations.OfType<DeleteDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)));
            Assert.NotEmpty(operations.OfType<DeleteDiffOperation>().Single(x => x.EntityName == nameof(EntityLevel0)).Keys);
            Assert.Empty(operations.OfType<DeleteDiffOperation>().Single(x => x.EntityName == nameof(EntityLevel0)).NavigationParentKeys); // no parent
            // EntityLevel1
            Assert.Equal(5, operations.OfType<DeleteDiffOperation>().Count(x => x.EntityName == nameof(EntityLevel1)));
            Assert.All(operations.OfType<DeleteDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.NotEmpty(x.Keys));
            Assert.All(operations.OfType<DeleteDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.NotEmpty(x.NavigationParentKeys));
            Assert.All(operations.OfType<DeleteDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.Single(x.NavigationParentKeys.Keys));
            Assert.All(operations.OfType<DeleteDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.NavigationParentKeys), x => Assert.Equal(nameof(EntityLevel0), x.Key)); // one parent of EnityLevel0
            Assert.All(operations.OfType<DeleteDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.NavigationParentKeys), x => Assert.Equal(2, x.Value.Count)); // EntityLevel0 has 2 keys (StartsOn and Direction)
        }

        [Theory]
        [InlineData(EqualityComparers.Precompiled, true)]
        [InlineData(EqualityComparers.Precompiled, false)]
        [InlineData(EqualityComparers.Naive, true)]
        [InlineData(EqualityComparers.Naive, false)]
        public void CompareMany(EqualityComparers equalityComparer, bool defineOperationsOnEntity)
        {
            var existingEntities = new List<EntityLevel0>();
            var newEntities = new List<EntityLevel0>();
            foreach (var index in Enumerable.Range(1, 10))
            {
                var (existingEntity, newEntity) = GenerateModifications(index);
                existingEntities.Add(existingEntity);
                newEntities.Add(newEntity);
            }

            var deepDiff = CreateDeepDiff(defineOperationsOnEntity);
            var listener = new StoreAllOperationListener();
            deepDiff.CompareMany(existingEntities, newEntities, listener, cfg => cfg.SetEqualityComparer(equalityComparer));
            var operations = listener.Operations;

            Assert.NotEmpty(operations);
            // 10 delete
            Assert.Equal(10, operations.OfType<DeleteDiffOperation>().Count());
            Assert.Equal(10, operations.OfType<DeleteDiffOperation>().Count(x => x.EntityName == nameof(EntityLevel1)));
            Assert.NotEmpty(operations.OfType<DeleteDiffOperation>().SelectMany(x => x.Keys));
            // 10 insert
            Assert.Equal(10, operations.OfType<InsertDiffOperation>().Count());
            Assert.Equal(10, operations.OfType<InsertDiffOperation>().Count(x => x.EntityName == nameof(EntityLevel1)));
            Assert.NotEmpty(operations.OfType<InsertDiffOperation>().SelectMany(x => x.Keys));
            // 10 update on EntityLevel0 and 40 updates on EntityLevel1
            // each update EntityLevel1 are on Power property -> generate 1 UpdatedProperty (Power)
            Assert.Equal(5*10, operations.OfType<UpdateDiffOperation>().Count());
            //
            Assert.Equal(10, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(EntityLevel0)));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)), x => Assert.Single(x.UpdatedProperties));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)), x => Assert.Equal(nameof(EntityLevel0.Penalty), x.UpdatedProperties.Single().PropertyName));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)), x => Assert.Empty(x.NavigationParentKeys)); // no parent
            //
            Assert.Equal(4*10, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(EntityLevel1)));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.Single(x.UpdatedProperties));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.Equal(nameof(EntityLevel1.Power), x.PropertyName));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.NotEqual(x.ExistingValue, x.NewValue));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.Equal(Convert.ToInt32(x.ExistingValue) * 2, Convert.ToInt32(x.NewValue)));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.Single(x.NavigationParentKeys)); // one parent
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.All(x.NavigationParentKeys, y => Assert.Equal(nameof(EntityLevel0), y.Key))); // one parent of EnityLevel0
        }

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

        private static (EntityLevel0 existingEntity, EntityLevel0 newEntity) GenerateModifications(int level0Index)
        {
            var existingEntity = GenerateExisting(level0Index);
            var newEntity = GenerateNew(level0Index);
            return (existingEntity, newEntity);
        }

        private static EntityLevel0 GenerateExisting(int level0Index)
            => new ()
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
    }
}
