using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Profile;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class DuplicateDiffEntityConfigurationExceptionTests
    {
        [Fact]
        public void DuplicateAddDiffConfiguration()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.StartsOn);

            Assert.Throws<DuplicateDiffEntityConfigurationException>(() => diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.StartsOn));
        }

        [Fact]
        public void DuplicateAddProfile()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.AddProfile<CapacityAvailabilityProfile>();

            Assert.Throws<DuplicateDiffEntityConfigurationException>(() => diffConfiguration.AddProfile<CapacityAvailabilityProfile>());
        }

        [Fact]
        public void DuplicateCreateDiffEntityConfiguration()
        {
            var diffConfiguration = new DiffConfiguration();

            Assert.Throws<DuplicateDiffEntityConfigurationException>(() => diffConfiguration.AddProfile<DuplicateCapacityAvailabilityProfile>());
        }
    }
}
