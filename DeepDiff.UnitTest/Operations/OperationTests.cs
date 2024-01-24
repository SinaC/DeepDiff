using DeepDiff.Configuration;
using DeepDiff.Operations;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeepDiff.UnitTest.Operations
{
    public class OperationTests
    {
        [Fact]
        public void GlobalDisableOperationsGeneration()
        {
            var (existingEntity, newEntity) = GenerateModificationInNested();

            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
                .HasMany(x => x.SubEntities);
            diffConfiguration.PersistEntity<EntityLevel1>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.Power, x.Price });

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.DiffSingle(existingEntity, newEntity, cfg => cfg.DisableOperationsGeneration());
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.Empty(operations);
        }

        [Fact]
        public void UpdateDisableOperationGeneration()
        {
            var (existingEntity, newEntity) = GenerateModificationInNested();

            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }).DisableOperationsGeneration())
                .HasMany(x => x.SubEntities);
            diffConfiguration.PersistEntity<EntityLevel1>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.Power, x.Price })
                .OnUpdate(cfg => cfg.DisableOperationsGeneration());

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.DiffSingle(existingEntity, newEntity);
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.Empty(operations.OfType<UpdateDiffOperation>());
            Assert.NotEmpty(operations.OfType<InsertDiffOperation>());
            Assert.NotEmpty(operations.OfType<DeleteDiffOperation>());
        }

        [Fact]
        public void InsertDisableOperationGeneration()
        {
            var (existingEntity, newEntity) = GenerateModificationInNested();

            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
                .OnInsert(cfg => cfg.DisableOperationsGeneration())
                .HasMany(x => x.SubEntities);
            diffConfiguration.PersistEntity<EntityLevel1>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.Power, x.Price })
                .OnInsert(cfg => cfg.DisableOperationsGeneration());

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.DiffSingle(existingEntity, newEntity);
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.NotEmpty(operations.OfType<UpdateDiffOperation>());
            Assert.Empty(operations.OfType<InsertDiffOperation>());
            Assert.NotEmpty(operations.OfType<DeleteDiffOperation>());
        }

        [Fact]
        public void DeleteDisableOperationGeneration()
        {
            var (existingEntity, newEntity) = GenerateModificationInNested();

            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => new { x.AdditionalValueToCopy }))
                .OnDelete(cfg => cfg.DisableOperationsGeneration())
                .HasMany(x => x.SubEntities);
            diffConfiguration.PersistEntity<EntityLevel1>()
                .HasKey(x => x.Timestamp)
                .HasValues(x => new { x.Power, x.Price })
                .OnDelete(cfg => cfg.DisableOperationsGeneration());

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.DiffSingle(existingEntity, newEntity);
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.NotEmpty(operations.OfType<UpdateDiffOperation>());
            Assert.NotEmpty(operations.OfType<InsertDiffOperation>());
            Assert.Empty(operations.OfType<DeleteDiffOperation>());
        }

        private (EntityLevel0 existingEntity, EntityLevel0 newEntity) GenerateModificationInNested()
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

            return (existingEntity,  newEntity);
        }
    }
}
