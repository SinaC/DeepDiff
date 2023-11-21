using EntityMerger.Configuration;
using EntityMerger.Exceptions;
using System.Linq;
using Xunit;

namespace EntityMerger.UnitTest.Exceptions
{
    public class MissingConfigurationExceptionTests
    {
        [Fact]
        public void MissingConfigurationException()
        {
            var mergeConfiguration = new MergeConfiguration();
            var merger = mergeConfiguration.CreateMerger();

            Assert.Throws<MissingConfigurationException>(() => merger.Merge(Enumerable.Empty<Entities.Simple.EntityLevel0>(), Enumerable.Empty<Entities.Simple.EntityLevel0>()).ToArray());
        }
    }
}
