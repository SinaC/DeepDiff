using DeepDiff.Configuration;
using DeepDiff.Operations;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Operations
{
    public class GenerateOperationsOnlyTests
    {
        [Fact]
        public void GlobalDisableOperationsGeneration()
        {
            var (existingEntity, newEntity) = GenerateModifications();

            var diffConfiguration = new DeepDiffConfiguration();
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
            var diff = deepDiff.DiffSingle(existingEntity, newEntity, cfg => cfg.GenerateOperations(false).OnlyGenerateOperations(true)); // meaning less
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.Null(result);
            Assert.Empty(operations);
        }

        [Fact]
        public void UpdateDisableOperationsGeneration()
        {
            var (existingEntity, newEntity) = GenerateModifications();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }).GenerateOperations(false))
                .HasMany(x => x.SubEntities);
            diffConfiguration.Entity<EntityLevel1>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.Power, x.Price })
                .OnUpdate(cfg => cfg.GenerateOperations(false));

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.DiffSingle(existingEntity, newEntity, cfg => cfg.OnlyGenerateOperations(true));
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.Null(result);
            Assert.Empty(operations.OfType<UpdateDiffOperation>());
            Assert.NotEmpty(operations.OfType<InsertDiffOperation>());
            Assert.NotEmpty(operations.OfType<DeleteDiffOperation>());
        }

        [Fact]
        public void InsertDisableOperationsGeneration()
        {
            var (existingEntity, newEntity) = GenerateModifications();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
                .OnInsert(cfg => cfg.GenerateOperations(false))
                .HasMany(x => x.SubEntities);
            diffConfiguration.Entity<EntityLevel1>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.Power, x.Price })
                .OnInsert(cfg => cfg.GenerateOperations(false));

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.DiffSingle(existingEntity, newEntity, cfg => cfg.OnlyGenerateOperations(true));
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.Null(result);
            Assert.NotEmpty(operations.OfType<UpdateDiffOperation>());
            Assert.Empty(operations.OfType<InsertDiffOperation>());
            Assert.NotEmpty(operations.OfType<DeleteDiffOperation>());
        }

        [Fact]
        public void DeleteDisableOperationsGeneration()
        {
            var (existingEntity, newEntity) = GenerateModifications();

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
                .OnDelete(cfg => cfg.GenerateOperations(false))
                .HasMany(x => x.SubEntities);
            diffConfiguration.Entity<EntityLevel1>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.Power, x.Price })
                .OnDelete(cfg => cfg.GenerateOperations(false));

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.DiffSingle(existingEntity, newEntity, cfg => cfg.OnlyGenerateOperations(true));
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.Null(result);
            Assert.NotEmpty(operations.OfType<UpdateDiffOperation>());
            Assert.NotEmpty(operations.OfType<InsertDiffOperation>());
            Assert.Empty(operations.OfType<DeleteDiffOperation>());
        }

        [Fact]
        public void CheckOperations()
        {
            var (existingEntity, newEntity) = GenerateModifications();

            var diffConfiguration = new DeepDiffConfiguration();
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
            var diff = deepDiff.DiffSingle(existingEntity, newEntity, cfg => cfg.OnlyGenerateOperations(true));
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.Null(result);
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
            // each update EntityLevel1 are on Power property -> generate 1 UpdatedProperty (Power), NO CopyValuesProperty (AdditionalValueToCopy) and 1 SetValueProperty (PersistChange)
            Assert.Equal(5, operations.OfType<UpdateDiffOperation>().Count());
            //
            Assert.Single(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)), x => Assert.Single(x.UpdatedProperties));
            Assert.Equal(nameof(EntityLevel0.Penalty), operations.OfType<UpdateDiffOperation>().Single(x => x.EntityName == nameof(EntityLevel0)).UpdatedProperties.Single().PropertyName);
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)), x => Assert.Single(x.CopyValuesProperties));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)).SelectMany(x => x.CopyValuesProperties), x => Assert.Equal(nameof(EntityLevel0.AdditionalValueToCopy), x.PropertyName));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)), x => Assert.Single(x.SetValueProperties));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)).SelectMany(x => x.SetValueProperties), x => Assert.Equal(nameof(EntityLevel0.PersistChange), x.PropertyName));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel0)).SelectMany(x => x.SetValueProperties), x => Assert.Equal(PersistChange.Update.ToString(), x.NewValue));
            //
            Assert.Equal(4, operations.OfType<UpdateDiffOperation>().Count(x => x.EntityName == nameof(EntityLevel1)));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.Single(x.UpdatedProperties));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.Equal(nameof(EntityLevel1.Power), x.PropertyName));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.NotEqual(x.ExistingValue, x.NewValue));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.UpdatedProperties), x => Assert.Equal(Convert.ToInt32(x.ExistingValue) * 2, Convert.ToInt32(x.NewValue)));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.Empty(x.CopyValuesProperties));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)), x => Assert.Single(x.SetValueProperties));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.SetValueProperties), x => Assert.Equal(nameof(EntityLevel1.PersistChange), x.PropertyName));
            Assert.All(operations.OfType<UpdateDiffOperation>().Where(x => x.EntityName == nameof(EntityLevel1)).SelectMany(x => x.SetValueProperties), x => Assert.Equal(PersistChange.Update.ToString(), x.NewValue));
        }

        private (EntityLevel0 existingEntity, EntityLevel0 newEntity) GenerateModifications()
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
                Penalty = 4,
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
}
