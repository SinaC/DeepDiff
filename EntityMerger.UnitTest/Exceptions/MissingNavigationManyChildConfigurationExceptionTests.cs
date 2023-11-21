using EntityMerger.Configuration;
using EntityMerger.Exceptions;
using Xunit;

namespace EntityMerger.UnitTest.Exceptions
{
    public class MissingNavigationManyChildConfigurationExceptionTests
    {
        [Fact]
        public void MissingNavigationManyChildConfigurationException()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => x.StartsOn)
                .HasMany(x => x.SubEntities);

            Assert.Throws<MissingNavigationManyChildConfigurationException>(() => mergeConfiguration.CreateMerger());
        }
    }
}
