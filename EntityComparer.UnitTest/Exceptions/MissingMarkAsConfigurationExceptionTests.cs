using EntityComparer.Configuration;
using EntityComparer.Exceptions;
using System;
using Xunit;

namespace EntityComparer.UnitTest.Exceptions
{
    public class MissingMarkAsConfigurationExceptionTests
    {
        [Fact]
        public void MissingInsertAndUpdateAndDelete()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction });

            var exception = Assert.Throws<AggregateException>(() => compareConfiguration.CreateComparer());
            Assert.Equal(3, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsType<MissingMarkAsConfigurationException>(x));
        }

        [Fact]
        public void MissingInsertAndUpdate()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsDeleted(x => x.PersistChange, Entities.PersistChange.Delete);

            var exception = Assert.Throws<AggregateException>(() => compareConfiguration.CreateComparer());
            Assert.Equal(2, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsType<MissingMarkAsConfigurationException>(x));
        }

        [Fact]
        public void MissingInsertAndDelete()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsUpdated(x => x.PersistChange, Entities.PersistChange.Update);

            var exception = Assert.Throws<AggregateException>(() => compareConfiguration.CreateComparer());
            Assert.Equal(2, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsType<MissingMarkAsConfigurationException>(x));
        }

        [Fact]
        public void MissingUpdateAndDelete()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsInserted(x => x.PersistChange, Entities.PersistChange.Insert);

            var exception = Assert.Throws<AggregateException>(() => compareConfiguration.CreateComparer());
            Assert.Equal(2, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsType<MissingMarkAsConfigurationException>(x));
        }

        [Fact]
        public void MissingInsert()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsUpdated(x => x.PersistChange, Entities.PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, Entities.PersistChange.Delete);

            Assert.Throws<MissingMarkAsConfigurationException>(() => compareConfiguration.CreateComparer());
        }

        [Fact]
        public void MissingUpdate()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsInserted(x => x.PersistChange, Entities.PersistChange.Insert)
                .MarkAsDeleted(x => x.PersistChange, Entities.PersistChange.Delete);

            Assert.Throws<MissingMarkAsConfigurationException>(() => compareConfiguration.CreateComparer());
        }

        [Fact]
        public void MissingDelete()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsInserted(x => x.PersistChange, Entities.PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, Entities.PersistChange.Update);

            Assert.Throws<MissingMarkAsConfigurationException>(() => compareConfiguration.CreateComparer());
        }
    }
}
