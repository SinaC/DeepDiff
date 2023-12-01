using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class InvalidNavigationOneChildTypeConfigurationExceptionTests
    {
        [Fact]
        public void HasOneOnCollection()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => x.StartsOn)
                .HasOne(x => x.SubEntities);

            Assert.Throws<InvalidNavigationOneChildTypeConfigurationException>(() => diffConfiguration.CreateDeepDiff());
        }

    }
}
