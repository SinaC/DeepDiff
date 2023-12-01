using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using System;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class MissingMarkAsConfigurationExceptionTests
    {
        [Fact]
        public void MissingInsertAndUpdateAndDelete()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction });

            var exception = Assert.Throws<AggregateException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Equal(3, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsType<MissingMarkAsConfigurationException>(x));
        }

        [Fact]
        public void MissingInsertAndUpdate()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsDeleted(x => x.PersistChange, Entities.PersistChange.Delete);

            var exception = Assert.Throws<AggregateException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Equal(2, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsType<MissingMarkAsConfigurationException>(x));
        }

        [Fact]
        public void MissingInsertAndDelete()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsUpdated(x => x.PersistChange, Entities.PersistChange.Update);

            var exception = Assert.Throws<AggregateException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Equal(2, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsType<MissingMarkAsConfigurationException>(x));
        }

        [Fact]
        public void MissingUpdateAndDelete()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsInserted(x => x.PersistChange, Entities.PersistChange.Insert);

            var exception = Assert.Throws<AggregateException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Equal(2, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsType<MissingMarkAsConfigurationException>(x));
        }

        [Fact]
        public void MissingInsert()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsUpdated(x => x.PersistChange, Entities.PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, Entities.PersistChange.Delete);

            Assert.Throws<MissingMarkAsConfigurationException>(() => diffConfiguration.CreateDeepDiff());
        }

        [Fact]
        public void MissingUpdate()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsInserted(x => x.PersistChange, Entities.PersistChange.Insert)
                .MarkAsDeleted(x => x.PersistChange, Entities.PersistChange.Delete);

            Assert.Throws<MissingMarkAsConfigurationException>(() => diffConfiguration.CreateDeepDiff());
        }

        [Fact]
        public void MissingDelete()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsInserted(x => x.PersistChange, Entities.PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, Entities.PersistChange.Update);

            Assert.Throws<MissingMarkAsConfigurationException>(() => diffConfiguration.CreateDeepDiff());
        }
    }
}
