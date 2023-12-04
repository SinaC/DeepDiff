using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using System;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class MissingOperationConfigurationExceptionTests
    {
        [Fact]
        public void MissingInsertAndUpdateAndDelete()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction });

            var exception = Assert.Throws<AggregateException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Equal(3, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsAssignableFrom<MissingOperationConfigurationException>(x));
        }

        [Fact]
        public void MissingInsertAndUpdate()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, Entities.PersistChange.Delete));

            var exception = Assert.Throws<AggregateException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Equal(2, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsAssignableFrom<MissingOperationConfigurationException>(x));
        }

        [Fact]
        public void MissingInsertAndDelete()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, Entities.PersistChange.Update));

            var exception = Assert.Throws<AggregateException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Equal(2, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsAssignableFrom<MissingOperationConfigurationException>(x));
        }

        [Fact]
        public void MissingUpdateAndDelete()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, Entities.PersistChange.Insert));

            var exception = Assert.Throws<AggregateException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Equal(2, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsAssignableFrom<MissingOperationConfigurationException>(x));
        }

        [Fact]
        public void MissingInsert()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, Entities.PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, Entities.PersistChange.Delete));

            Assert.Throws<MissingOnInsertConfigurationException>(() => diffConfiguration.CreateDeepDiff());
        }

        [Fact]
        public void MissingUpdate()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, Entities.PersistChange.Insert))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, Entities.PersistChange.Delete));

            Assert.Throws<MissingOnUpdateConfigurationException>(() => diffConfiguration.CreateDeepDiff());
        }

        [Fact]
        public void MissingDelete()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, Entities.PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, Entities.PersistChange.Update));

            Assert.Throws<MissingOnDeleteConfigurationException>(() => diffConfiguration.CreateDeepDiff());
        }
    }
}
