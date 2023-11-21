using EntityMerger.Configuration;
using EntityMerger.Exceptions;
using Xunit;

namespace EntityMerger.UnitTest.Exceptions
{
    public class InvalidNavigationOneChildTypeConfigurationExceptionTests
    {
        [Fact]
        public void HasOneOnCollection()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasKey(x => x.StartsOn)
                .HasOne(x => x.SubEntities);

            Assert.Throws<InvalidNavigationOneChildTypeConfigurationException>(() => mergeConfiguration.CreateMerger());
        }

    }
}
