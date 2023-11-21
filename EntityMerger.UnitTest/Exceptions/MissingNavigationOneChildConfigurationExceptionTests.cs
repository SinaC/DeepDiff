using EntityMerger.Configuration;
using EntityMerger.Exceptions;
using Xunit;

namespace EntityMerger.UnitTest.Exceptions
{
    public class MissingNavigationOneChildConfigurationExceptionTests
    {
        [Fact]
        public void MissingNavigationOneChildConfigurationException()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => x.StartsOn)
                .HasOne(x => x.SubEntity);

            Assert.Throws<MissingNavigationOneChildConfigurationException>(() => mergeConfiguration.CreateMerger());
        }
    }
}
