using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class WithComparerTests
    {
        [Fact]
        public void MultipleProperties()
        {
            var diffConfiguration = new DiffConfiguration();
            Assert.Throws<InvalidOperationException>(() => diffConfiguration.Entity<Entities.Simple.EntityLevel2>()
                .WithComparer(x => new { x.Value1, x.Value2 }, new DecimalComparer(6)));
        }

        [Fact(Skip = "No check on type for custom converter")]
        public void WrongTypeConverter()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.RequestedPower, x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => x.AdditionalValueToCopy))
                .WithComparer<decimal>(new OrdinalIgnoreCaseStringComparer());

            var existingEntity = new EntityLevel0
            {

                Id = Guid.NewGuid(),
                StartsOn = DateTime.Today,
                Direction = Direction.Up,
                RequestedPower = 0,
                Penalty = 1,
            };

            var newEntity = new EntityLevel0
            {

                Id = Guid.NewGuid(),
                StartsOn = DateTime.Today,
                Direction = Direction.Up,
                RequestedPower = 10, // only difference
                Penalty = 1,
            };

            var deepDiff = diffConfiguration.CreateDeepDiff();
            var diff = deepDiff.DiffSingle(existingEntity, newEntity);
            var result = diff.Entity;

            Assert.NotNull(result);
        }
    }
}
