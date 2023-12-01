using DeepDiff.Configuration;
using DeepDiff.Exceptions;
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
            diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.StartsOn);

            Assert.Throws<DuplicateDiffEntityConfigurationException>(() => diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
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
