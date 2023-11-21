using EntityMerger.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace EntityMerger.UnitTest.Exceptions
{
    public class InvalidOperationExceptionTests
    {
        [Fact]
        public void SelfDefinedKey()
        {
            var mergeConfiguration = new MergeConfiguration();
            var mergeEntityConfiguration = mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => mergeEntityConfiguration.HasKey(x => x));
        }

        [Fact]
        public void SelfDefinedValues()
        {
            var mergeConfiguration = new MergeConfiguration();
            var mergeEntityConfiguration = mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => mergeEntityConfiguration.HasValues(x => x));
        }

        [Fact]
        public void SelfDefinedAdditionalValuesToCopy()
        {
            var mergeConfiguration = new MergeConfiguration();
            var mergeEntityConfiguration = mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => mergeEntityConfiguration.HasAdditionalValuesToCopy(x => x));
        }

        [Fact]
        public void SelfDefinedNavigationMany()
        {
            var mergeConfiguration = new MergeConfiguration();
            var mergeEntityConfiguration = mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => mergeEntityConfiguration.HasMany(x => new List<Entities.Simple.EntityLevel0> { x }));
        }

        [Fact]
        public void SelfDefinedNavigationOne()
        {
            var mergeConfiguration = new MergeConfiguration();
            var mergeEntityConfiguration = mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => mergeEntityConfiguration.HasOne(x => x));
        }
    }
}
