using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class MissingConfigurationExceptionTests
    {
        [Fact]
        public void MissingConfigurationException()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            var deepDiff = diffConfiguration.CreateDeepDiff();

            Assert.Throws<MissingConfigurationException>(() => deepDiff.MergeMany(Enumerable.Empty<Entities.Simple.EntityLevel0>(), Enumerable.Empty<Entities.Simple.EntityLevel0>()).Entities.ToArray());
        }
    }
}
