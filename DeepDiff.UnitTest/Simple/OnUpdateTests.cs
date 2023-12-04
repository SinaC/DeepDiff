using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities.Simple;
using System.Linq;
using System;
using Xunit;
using DeepDiff.UnitTest.Entities;

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

            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .OnUpdate(cfg => cfg
                    .SetValue(x => x.PersistChange, PersistChange.Update)
                    .CopyValues(x => x.AdditionalValueToCopy));

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var result = deepDiff.DiffSingle(existingEntity, newEntity);
            Assert.NotNull(result);
            Assert.Equal(PersistChange.Update, result.PersistChange);
            Assert.Equal(3, result.RequestedPower);
            Assert.Equal(7, result.Penalty);
            Assert.Equal("Existing", result.Comment); // comment is not copied
        }
    }
}
