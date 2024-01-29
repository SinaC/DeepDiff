﻿using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class DuplicateValuesConfigurationExceptionTests
    {
        [Fact]
        public void DuplicateHasValues_OnDifferentValues()
        {
            var diffConfiguration = new DiffConfiguration();
            var diffEntityConfiguration = diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.RequestedPower);

            Assert.Throws<DuplicateValuesConfigurationException>(() => diffEntityConfiguration.HasValues(x => x.Id));
        }

        [Fact]
        public void DuplicateHasValues_OnSameValues()
        {
            var diffConfiguration = new DiffConfiguration();
            var diffEntityConfiguration = diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.RequestedPower);

            Assert.Throws<DuplicateValuesConfigurationException>(() => diffEntityConfiguration.HasValues(x => x.RequestedPower));
        }
    }
}
