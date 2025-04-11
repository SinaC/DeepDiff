using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class DuplicateKeyConfigurationExceptionTests
    {
        [Fact]
        public void DuplicateHasKey_OnDifferentKey()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            var entityConfiguration = diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.RequestedPower);

            Assert.Throws<DuplicateKeyConfigurationException>(() => entityConfiguration.HasKey(x => x.Id));
        }

        [Fact]
        public void DuplicateHasKey_OnSameKey()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            var entityConfiguration = diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.RequestedPower);

            Assert.Throws<DuplicateKeyConfigurationException>(() => entityConfiguration.HasKey(x => new { x.StartsOn, x.Direction }));
        }
    }
}
