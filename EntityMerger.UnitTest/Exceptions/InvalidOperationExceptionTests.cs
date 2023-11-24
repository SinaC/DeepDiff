using EntityMerger.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void KeyOnNonProperty()
        {
            var mergeConfiguration = new MergeConfiguration();
            var mergeEntityConfiguration = mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => mergeEntityConfiguration.HasKey(x => 7));
        }

        [Fact]
        public void SelfDefinedValues()
        {
            var mergeConfiguration = new MergeConfiguration();
            var mergeEntityConfiguration = mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => mergeEntityConfiguration.HasValues(x => x));
        }

        [Fact]
        public void ValuesOnNonProperty()
        {
            var mergeConfiguration = new MergeConfiguration();
            var mergeEntityConfiguration = mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => mergeEntityConfiguration.HasValues(x => 7));
        }

        [Fact]
        public void SelfDefinedAdditionalValuesToCopy()
        {
            var mergeConfiguration = new MergeConfiguration();
            var mergeEntityConfiguration = mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => mergeEntityConfiguration.HasAdditionalValuesToCopy(x => x));
        }

        [Fact]
        public void AdditionalValuesToCopyOnNonProperty()
        {
            var mergeConfiguration = new MergeConfiguration();
            var mergeEntityConfiguration = mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => mergeEntityConfiguration.HasAdditionalValuesToCopy(x => 7));
        }

        [Fact]
        public void SelfDefinedNavigationMany()
        {
            var mergeConfiguration = new MergeConfiguration();
            var mergeEntityConfiguration = mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => mergeEntityConfiguration.HasMany(x => new List<Entities.Simple.EntityLevel0> { x }));
        }

        [Fact]
        public void NavigationManyOnNonPropertyCollection()
        {
            var mergeConfiguration = new MergeConfiguration();
            var mergeEntityConfiguration = mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => mergeEntityConfiguration.HasMany(x => Enumerable.Range(0, 5).Select(x => x.ToString()).ToList()));
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
