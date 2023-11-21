using EntityMerger.Configuration;
using EntityMerger.Exceptions;
using Xunit;

namespace EntityMerger.UnitTest.Exceptions
{
    public class MissingKeyConfigurationExceptionTests
    {
        [Fact]
        public void NoKey()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration.PersistEntity<Entities.Simple.EntityLevel0>()
                .HasValues(x => x.StartsOn);

            Assert.Throws<MissingKeyConfigurationException>(() => mergeConfiguration.CreateMerger());
        }
    }
}
