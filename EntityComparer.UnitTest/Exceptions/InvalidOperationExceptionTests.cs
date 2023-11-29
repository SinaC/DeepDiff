using EntityComparer.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EntityComparer.UnitTest.Exceptions
{
    public class InvalidOperationExceptionTests
    {
        [Fact]
        public void SelfDefinedKey()
        {
            var compareConfiguration = new CompareConfiguration();
            var compareEntityConfiguration = compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => compareEntityConfiguration.HasKey(x => x));
        }

        [Fact]
        public void KeyOnNonProperty()
        {
            var compareConfiguration = new CompareConfiguration();
            var compareEntityConfiguration = compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => compareEntityConfiguration.HasKey(x => 7));
        }

        [Fact]
        public void SelfDefinedValues()
        {
            var compareConfiguration = new CompareConfiguration();
            var compareEntityConfiguration = compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => compareEntityConfiguration.HasValues(x => x));
        }

        [Fact]
        public void ValuesOnNonProperty()
        {
            var compareConfiguration = new CompareConfiguration();
            var compareEntityConfiguration = compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => compareEntityConfiguration.HasValues(x => 7));
        }

        [Fact]
        public void SelfDefinedAdditionalValuesToCopy()
        {
            var compareConfiguration = new CompareConfiguration();
            var compareEntityConfiguration = compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => compareEntityConfiguration.HasAdditionalValuesToCopy(x => x));
        }

        [Fact]
        public void AdditionalValuesToCopyOnNonProperty()
        {
            var compareConfiguration = new CompareConfiguration();
            var compareEntityConfiguration = compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => compareEntityConfiguration.HasAdditionalValuesToCopy(x => 7));
        }

        [Fact]
        public void SelfDefinedNavigationMany()
        {
            var compareConfiguration = new CompareConfiguration();
            var compareEntityConfiguration = compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => compareEntityConfiguration.HasMany(x => new List<Entities.Simple.EntityLevel0> { x }));
        }

        [Fact]
        public void NavigationManyOnNonPropertyCollection()
        {
            var compareConfiguration = new CompareConfiguration();
            var compareEntityConfiguration = compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => compareEntityConfiguration.HasMany(x => Enumerable.Range(0, 5).Select(x => x.ToString()).ToList()));
        }

        [Fact]
        public void SelfDefinedNavigationOne()
        {
            var compareConfiguration = new CompareConfiguration();
            var compareEntityConfiguration = compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => compareEntityConfiguration.HasOne(x => x));
        }
    }
}
