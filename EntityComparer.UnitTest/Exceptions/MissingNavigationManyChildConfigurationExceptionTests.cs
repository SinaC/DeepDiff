using EntityComparer.Configuration;
using EntityComparer.Exceptions;
using Xunit;

namespace EntityComparer.UnitTest.Exceptions
{
    public class MissingNavigationManyChildConfigurationExceptionTests
    {
        [Fact]
        public void MissingNavigationManyChildConfigurationException()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => x.StartsOn)
                .HasMany(x => x.SubEntities);

            Assert.Throws<MissingNavigationManyChildConfigurationException>(() => compareConfiguration.CreateComparer());
        }
    }
}
