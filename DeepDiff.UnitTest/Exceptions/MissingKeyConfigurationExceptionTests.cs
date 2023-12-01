using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class MissingKeyConfigurationExceptionTests
    {
        [Fact]
        public void NoKey()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasValues(x => x.StartsOn);

            Assert.Throws<MissingKeyConfigurationException>(() => diffConfiguration.CreateDeepDiff());
        }
    }
}
