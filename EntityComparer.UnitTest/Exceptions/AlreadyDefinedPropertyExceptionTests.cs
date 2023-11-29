using EntityComparer.Configuration;
using EntityComparer.Exceptions;
using Xunit;

namespace EntityComparer.UnitTest.Exceptions
{
    public class AlreadyDefinedPropertyExceptionTests
    {
        [Fact]
        public void Values_AlsoInKey()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.StartsOn);

            Assert.Throws<AlreadyDefinedPropertyException>(() => compareConfiguration.CreateComparer());
        }

        [Fact]
        public void AdditionalValuesToCopy_AlsoInKey()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasAdditionalValuesToCopy(x => x.StartsOn);

            Assert.Throws<AlreadyDefinedPropertyException>(() => compareConfiguration.CreateComparer());
        }

        [Fact]
        public void AdditionalValuesToCopy_AlsoInValues()
        {
            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.Penalty })
                .HasAdditionalValuesToCopy(x => x.Penalty);

            Assert.Throws<AlreadyDefinedPropertyException>(() => compareConfiguration.CreateComparer());
        }
    }
}
