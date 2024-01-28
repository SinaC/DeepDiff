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
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.StartsOn)
                .HasOne(x => x.SubEntities);

            Assert.Throws<InvalidNavigationOneChildTypeConfigurationException>(() => diffConfiguration.CreateDeepDiff());
        }

    }
}
