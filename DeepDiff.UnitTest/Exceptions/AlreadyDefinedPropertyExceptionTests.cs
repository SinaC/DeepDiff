using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class AlreadyDefinedPropertyExceptionTests
    {
        [Fact]
        public void Values_AlsoInKey()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.StartsOn);

            Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
        }

        [Fact]
        public void AdditionalValuesToCopy_AlsoInKey()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasAdditionalValuesToCopy(x => x.StartsOn);

            Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
        }

        [Fact]
        public void AdditionalValuesToCopy_AlsoInValues()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.Penalty })
                .HasAdditionalValuesToCopy(x => x.Penalty);

            Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
        }
    }
}
