using EntityComparer.Configuration;
using EntityComparer.Exceptions;
using Xunit;

namespace EntityComparer.UnitTest.Exceptions
{
    public class InvalidNavigationOneChildTypeConfigurationExceptionTests
    {
        [Fact]
        public void HasOneOnCollection()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => x.StartsOn)
                .HasOne(x => x.SubEntities);

            Assert.Throws<InvalidNavigationOneChildTypeConfigurationException>(() => compareConfiguration.CreateComparer());
        }

    }
}
