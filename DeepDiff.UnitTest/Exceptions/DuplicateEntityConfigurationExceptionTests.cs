using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Profile;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class DuplicateEntityConfigurationExceptionTests
    {
        [Fact]
        public void DuplicateAddDiffConfiguration()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.StartsOn);

            Assert.Throws<DuplicateEntityConfigurationException>(() => diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.StartsOn));
        }

        [Fact]
        public void DuplicateAddProfile()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.AddProfile<CapacityAvailabilityProfile>();

            Assert.Throws<DuplicateEntityConfigurationException>(() => diffConfiguration.AddProfile<CapacityAvailabilityProfile>());
        }

        [Fact]
        public void DuplicateCreateEntityConfiguration()
        {
            var diffConfiguration = new DeepDiffConfiguration();

            Assert.Throws<DuplicateEntityConfigurationException>(() => diffConfiguration.AddProfile<DuplicateCapacityAvailabilityProfile>());
        }
    }
}
