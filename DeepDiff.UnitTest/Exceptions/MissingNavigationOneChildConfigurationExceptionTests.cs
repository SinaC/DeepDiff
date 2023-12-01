using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class MissingNavigationOneChildConfigurationExceptionTests
    {
        [Fact]
        public void MissingNavigationOneChildConfigurationException()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => x.StartsOn)
                .HasOne(x => x.SubEntity);

            Assert.Throws<MissingNavigationOneChildConfigurationException>(() => diffConfiguration.CreateDeepDiff());
        }
    }
}
