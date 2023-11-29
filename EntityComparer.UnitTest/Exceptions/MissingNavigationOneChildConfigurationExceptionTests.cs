using EntityComparer.Configuration;
using EntityComparer.Exceptions;
using Xunit;

namespace EntityComparer.UnitTest.Exceptions
{
    public class MissingNavigationOneChildConfigurationExceptionTests
    {
        [Fact]
        public void MissingNavigationOneChildConfigurationException()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => x.StartsOn)
                .HasOne(x => x.SubEntity);

            Assert.Throws<MissingNavigationOneChildConfigurationException>(() => compareConfiguration.CreateComparer());
        }
    }
}
