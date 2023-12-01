using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class MissingNavigationManyChildConfigurationExceptionTests
    {
        [Fact]
        public void MissingNavigationManyChildConfigurationException()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => x.StartsOn)
                .HasMany(x => x.SubEntities);

            Assert.Throws<MissingNavigationManyChildConfigurationException>(() => diffConfiguration.CreateDeepDiff());
        }
    }
}
