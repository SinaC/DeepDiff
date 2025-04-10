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

            Assert.Throws<MissingConfigurationException>(() => deepDiff.MergeMany(Enumerable.Range(0, 1).Select(x => new Entities.Simple.EntityLevel0()), Enumerable.Range(0, 1).Select(x => new Entities.Simple.EntityLevel0())).ToArray());
        }
    }
}
