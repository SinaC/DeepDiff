using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Simple
{
    public class OnUpdateTests
    {
        [Fact]
        public void OnUpdate_SetValue()
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
                RequestedPower = 3,
                Penalty = 7,
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

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .OnUpdate(cfg => cfg
                    .SetValue(x => x.PersistChange, PersistChange.Update)
                    .CopyValues(x => x.AdditionalValueToCopy));

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.MergeSingle(existingEntity, newEntity);
            var result = diff.Entity;

            Assert.NotNull(result);
            Assert.Equal(PersistChange.Update, result.PersistChange);
            Assert.Equal(3, result.RequestedPower);
            Assert.Equal(7, result.Penalty);
            Assert.Equal("NewAdditionalValue", result.AdditionalValueToCopy);
            Assert.Equal("Existing", result.Comment); // comment is not copied
        }

        [Fact]
        public void OnUpdate_SetValue_Naive()
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
                RequestedPower = 3,
                Penalty = 7,
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

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .OnUpdate(cfg => cfg
                    .SetValue(x => x.PersistChange, PersistChange.Update)
                    .CopyValues(x => x.AdditionalValueToCopy));

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.MergeSingle(existingEntity, newEntity, cfg => cfg.UsePrecompiledEqualityComparer(false));
            var result = diff.Entity;

            Assert.NotNull(result);
            Assert.Equal(PersistChange.Update, result.PersistChange);
            Assert.Equal(3, result.RequestedPower);
            Assert.Equal(7, result.Penalty);
            Assert.Equal("NewAdditionalValue", result.AdditionalValueToCopy);
            Assert.Equal("Existing", result.Comment); // comment is not copied
        }

        [Fact]
        public void OnUpdate_MultipleSetValue()
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
                RequestedPower = 3,
                Penalty = 7,
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

            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .OnUpdate(cfg => cfg
                    .SetValue(x => x.PersistChange, PersistChange.Update)
                    .SetValue(x => x.Comment, "THIS IS A COMMENT")
                    .CopyValues(x => x.AdditionalValueToCopy));

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.MergeSingle(existingEntity, newEntity);
            var result = diff.Entity;

            Assert.NotNull(result);
            Assert.Equal(PersistChange.Update, result.PersistChange);
            Assert.Equal(3, result.RequestedPower);
            Assert.Equal(7, result.Penalty);
            Assert.Equal("NewAdditionalValue", result.AdditionalValueToCopy);
            Assert.Equal("THIS IS A COMMENT", result.Comment); // comment is set to THIS IS A COMMENT
        }
    }
}
