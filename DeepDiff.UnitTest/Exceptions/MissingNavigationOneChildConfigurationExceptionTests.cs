using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class MissingNavigationOneChildConfigurationExceptionTests
    {
        [Fact]
        public void MissingNavigationOneChildConfigurationException()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.StartsOn)
                .HasOne(x => x.SubEntity);

            Assert.Throws<MissingNavigationOneChildConfigurationException>(() => diffConfiguration.CreateDeepDiff());
        }
    }
}
