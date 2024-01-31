using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class InvalidOperationExceptionTests
    {
        [Fact]
        public void SelfDefinedKey()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            var entityConfiguration = diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));

            Assert.Throws<InvalidOperationException>(() => entityConfiguration.HasKey(x => x));
        }

        [Fact]
        public void KeyOnNonProperty()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            var entityConfiguration = diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));

            Assert.Throws<InvalidOperationException>(() => entityConfiguration.HasKey(x => 7));
        }

        [Fact]
        public void SelfDefinedValues()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            var entityConfiguration = diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));

            Assert.Throws<InvalidOperationException>(() => entityConfiguration.HasValues(x => x));
        }

        [Fact]
        public void ValuesOnNonProperty()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            var entityConfiguration = diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));

            Assert.Throws<InvalidOperationException>(() => entityConfiguration.HasValues(x => 7));
        }

        [Fact]
        public void SelfDefinedAdditionalValuesToCopy()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            var entityConfiguration = diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));

            Assert.Throws<InvalidOperationException>(() => entityConfiguration.OnUpdate(cfg => cfg.CopyValues(x => x)));
        }

        [Fact]
        public void AdditionalValuesToCopyOnNonProperty()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            var entityConfiguration = diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));

            Assert.Throws<InvalidOperationException>(() => entityConfiguration.OnUpdate(cfg => cfg.CopyValues(x => 7)));
        }

        [Fact]
        public void SelfDefinedNavigationMany()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            var entityConfiguration = diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));

            Assert.Throws<InvalidOperationException>(() => entityConfiguration.HasMany(x => new List<Entities.Simple.EntityLevel0> { x }));
        }

        [Fact]
        public void NavigationManyOnNonPropertyCollection()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            var entityConfiguration = diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));

            Assert.Throws<InvalidOperationException>(() => entityConfiguration.HasMany(x => Enumerable.Range(0, 5).Select(x => x.ToString()).ToList()));
        }

        [Fact]
        public void SelfDefinedNavigationOne()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            var entityConfiguration = diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));

            Assert.Throws<InvalidOperationException>(() => entityConfiguration.HasOne(x => x));
        }
    }
}
