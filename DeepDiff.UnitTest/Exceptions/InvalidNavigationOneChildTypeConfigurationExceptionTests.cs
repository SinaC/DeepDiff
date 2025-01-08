using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class InvalidNavigationOneChildTypeConfigurationExceptionTests
    {
        [Fact]
        public void HasOneOnCollection()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.StartsOn)
                .HasOne(x => x.SubEntities);

            Assert.Throws<InvalidNavigationOneChildTypeConfigurationException>(() => diffConfiguration.CreateDeepDiff());
        }

        [Fact]
        public void HasOneOnAbstract()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<ParentEntity>()
                .HasKey(x => x.Key)
                .HasOne(x => x.Child);
            diffConfiguration.Entity<ChildEntity1>()
                .NoKey()
                .HasValues(x => x.Name);

            Assert.Throws<InvalidNavigationOneChildTypeConfigurationException>(() => diffConfiguration.CreateDeepDiff());
        }

        internal class ParentEntity
        {
            public int Key { get; set; }

            public ChildEntityBase Child { get; set; } = null!;
        }

        internal abstract class ChildEntityBase
        {
        }

        internal class ChildEntity1 : ChildEntityBase
        {
            public string Name { get; set; } = null!;
        }
    }
}
