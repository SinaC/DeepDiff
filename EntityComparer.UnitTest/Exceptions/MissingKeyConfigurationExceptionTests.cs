using EntityComparer.Configuration;
using EntityComparer.Exceptions;
using Xunit;

namespace EntityComparer.UnitTest.Exceptions
{
    public class MissingKeyConfigurationExceptionTests
    {
        [Fact]
        public void NoKey()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasValues(x => x.StartsOn);

            Assert.Throws<MissingKeyConfigurationException>(() => compareConfiguration.CreateComparer());
        }
    }
}
