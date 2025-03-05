using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities.Simple;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class NoKeyAndKeyConfigurationExceptionTests
    {
        [Fact]
        public void NoKeyThenHasKeyConfiguration()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            var entity0Config = diffConfiguration.Entity<EntityLevel0>()
                .NoKey();

            Assert.Throws<NoKeyAndHasKeyConfigurationException>(() => entity0Config.HasKey(x => x.StartsOn));
        }

        [Fact]
        public void HasKeyThenNoKeyConfiguration()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            var entity0Config = diffConfiguration.Entity<EntityLevel0>()
                .HasKey(x => x.StartsOn);

            Assert.Throws<NoKeyAndHasKeyConfigurationException>(() => entity0Config.NoKey());
        }
    }
}
