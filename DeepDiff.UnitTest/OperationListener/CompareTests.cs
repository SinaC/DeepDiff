using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.OperationListener
{
    public class CompareTests
    {
        [Fact]
        public void CompareSingle()
        {
            var (existingEntity, newEntity) = GenerateModifications(1);

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
                .HasMany(x => x.SubEntities)
                .Ignore(x => x.Index);
            diffConfiguration.Entity<EntityLevel1>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.Power, x.Price })
                .Ignore(x => x.Index);

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var listener = new StoreAllOperationListener();
            deepDiff.CompareSingle(existingEntity, newEntity, listener);
            var operations = listener.Operations;

            Assert.NotEmpty(operations);
            // 1 delete
            Assert.Single(operations.OfType<DeleteDiffOperation>());
            Assert.Single(operations.OfType<DeleteDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)));
            Assert.NotEmpty(operations.OfType<DeleteDiffOperation>().Single().Keys);
            // 1 insert
            Assert.Single(operations.OfType<InsertDiffOperation>());
            Assert.Single(operations.OfType<InsertDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)));
            Assert.NotEmpty(operations.OfType<InsertDiffOperation>().Single().Keys);
            // 1 update on EntityLevel0 and 4 updates on EntityLevel1
            // each update EntityLevel1 are on Power property -> generate 1 UpdatedProperty (Power)
            Assert.Equal(5, operations.OfType<UpdateDiffOperation>().Count());
            //
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)), x => Assert.Single(x.UpdatedProperties));
            Assert.Equal(nameof(EntityLevel0.Penalty), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(EntityLevel0)).UpdatedProperties.Single().PropertyName);
            //
            Assert.Equal(4, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(EntityLevel1)));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.Single(x.UpdatedProperties));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.Equal(nameof(EntityLevel1.Power), x.PropertyName));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.NotEqual(x.ExistingValue, x.NewValue));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.Equal(Convert.ToInt32(x.ExistingValue) * 2, Convert.ToInt32(x.NewValue)));
        }

        [Fact]
        public void CompareMany()
        {
            var existingEntities = new List<EntityLevel0>();
            var newEntities = new List<EntityLevel0>();
            foreach (var index in Enumerable.Range(1, 10))
            {
                var (existingEntity, newEntity) = GenerateModifications(index);
                existingEntities.Add(existingEntity);
                newEntities.Add(newEntity);
            }

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
                .HasMany(x => x.SubEntities)
                .Ignore(x => x.Index);
            diffConfiguration.Entity<EntityLevel1>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.Power, x.Price })
                .Ignore(x => x.Index);

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var listener = new StoreAllOperationListener();
            deepDiff.CompareMany(existingEntities, newEntities, listener);
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
            //
            Assert.Equal(4*10, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(EntityLevel1)));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.Single(x.UpdatedProperties));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.Equal(nameof(EntityLevel1.Power), x.PropertyName));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.NotEqual(x.ExistingValue, x.NewValue));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.Equal(Convert.ToInt32(x.ExistingValue) * 2, Convert.ToInt32(x.NewValue)));
        }

        private (EntityLevel0 existingEntity, EntityLevel0 newEntity) GenerateModifications(int level0Index)
        {
            var existingEntity = new EntityLevel0
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

            var newEntity = new EntityLevel0
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

            return (existingEntity, newEntity);
        }
    }
}
