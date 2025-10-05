using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities.Simple;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class NoKeyButFoundInNavigationManyConfigurationExceptionTests
    {
        [Fact]
        public void NoKeyAndInHasManyOfAnotherConfiguration()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<EntityLevel0>()
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasMany(x => x.SubEntities);
            diffConfiguration.ConfigureEntity<EntityLevel1>()
                .NoKey();

            var exception = Assert.Throws<NoKeyButFoundInNavigationManyConfigurationException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Single(exception.ReferencingEntities);
            Assert.Equal(typeof(EntityLevel0), exception.ReferencingEntities.Single());
        }
    }
}
