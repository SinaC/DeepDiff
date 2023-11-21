using EntityMerger.Configuration;
using EntityMerger.Exceptions;
using System;
using Xunit;

namespace EntityMerger.UnitTest.Exceptions
{
    public class MissingMarkAsConfigurationExceptionTests
    {
        [Fact]
        public void MissingInsertAndUpdateAndDelete()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction });

            var exception = Assert.Throws<AggregateException>(() => mergeConfiguration.CreateMerger());
            Assert.Equal(3, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsType<MissingMarkAsConfigurationException>(x));
        }

        [Fact]
        public void MissingInsertAndUpdate()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsDeleted(x => x.PersistChange, Entities.PersistChange.Delete);

            var exception = Assert.Throws<AggregateException>(() => mergeConfiguration.CreateMerger());
            Assert.Equal(2, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsType<MissingMarkAsConfigurationException>(x));
        }

        [Fact]
        public void MissingInsertAndDelete()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsUpdated(x => x.PersistChange, Entities.PersistChange.Update);

            var exception = Assert.Throws<AggregateException>(() => mergeConfiguration.CreateMerger());
            Assert.Equal(2, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsType<MissingMarkAsConfigurationException>(x));
        }

        [Fact]
        public void MissingUpdateAndDelete()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsInserted(x => x.PersistChange, Entities.PersistChange.Insert);

            var exception = Assert.Throws<AggregateException>(() => mergeConfiguration.CreateMerger());
            Assert.Equal(2, exception.InnerExceptions.Count);
            Assert.All(exception.InnerExceptions, x => Assert.IsType<MissingMarkAsConfigurationException>(x));
        }

        [Fact]
        public void MissingInsert()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsUpdated(x => x.PersistChange, Entities.PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, Entities.PersistChange.Delete);

            Assert.Throws<MissingMarkAsConfigurationException>(() => mergeConfiguration.CreateMerger());
        }

        [Fact]
        public void MissingUpdate()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsInserted(x => x.PersistChange, Entities.PersistChange.Insert)
                .MarkAsDeleted(x => x.PersistChange, Entities.PersistChange.Delete);

            Assert.Throws<MissingMarkAsConfigurationException>(() => mergeConfiguration.CreateMerger());
        }

        [Fact]
        public void MissingDelete()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .MarkAsInserted(x => x.PersistChange, Entities.PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, Entities.PersistChange.Update);

            Assert.Throws<MissingMarkAsConfigurationException>(() => mergeConfiguration.CreateMerger());
        }
    }
}
