using DeepDiff.Configuration;
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
            var diffConfiguration = new DiffConfiguration();
            var diffEntityConfiguration = diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => diffEntityConfiguration.HasKey(x => x));
        }

        [Fact]
        public void KeyOnNonProperty()
        {
            var diffConfiguration = new DiffConfiguration();
            var diffEntityConfiguration = diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => diffEntityConfiguration.HasKey(x => 7));
        }

        [Fact]
        public void SelfDefinedValues()
        {
            var diffConfiguration = new DiffConfiguration();
            var diffEntityConfiguration = diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => diffEntityConfiguration.HasValues(x => x));
        }

        [Fact]
        public void ValuesOnNonProperty()
        {
            var diffConfiguration = new DiffConfiguration();
            var diffEntityConfiguration = diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => diffEntityConfiguration.HasValues(x => 7));
        }

        [Fact]
        public void SelfDefinedAdditionalValuesToCopy()
        {
            var diffConfiguration = new DiffConfiguration();
            var diffEntityConfiguration = diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => diffEntityConfiguration.OnUpdate(cfg => cfg.CopyValues(x => x)));
        }

        [Fact]
        public void AdditionalValuesToCopyOnNonProperty()
        {
            var diffConfiguration = new DiffConfiguration();
            var diffEntityConfiguration = diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => diffEntityConfiguration.OnUpdate(cfg => cfg.CopyValues(x => 7)));
        }

        [Fact]
        public void SelfDefinedNavigationMany()
        {
            var diffConfiguration = new DiffConfiguration();
            var diffEntityConfiguration = diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => diffEntityConfiguration.HasMany(x => new List<Entities.Simple.EntityLevel0> { x }));
        }

        [Fact]
        public void NavigationManyOnNonPropertyCollection()
        {
            var diffConfiguration = new DiffConfiguration();
            var diffEntityConfiguration = diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => diffEntityConfiguration.HasMany(x => Enumerable.Range(0, 5).Select(x => x.ToString()).ToList()));
        }

        [Fact]
        public void SelfDefinedNavigationOne()
        {
            var diffConfiguration = new DiffConfiguration();
            var diffEntityConfiguration = diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>();

            Assert.Throws<InvalidOperationException>(() => diffEntityConfiguration.HasOne(x => x));
        }
    }
}
