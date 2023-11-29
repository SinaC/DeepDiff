using EntityComparer.Configuration;
using EntityComparer.Exceptions;
using System.Linq;
using Xunit;

namespace EntityComparer.UnitTest.Exceptions
{
    public class MissingConfigurationExceptionTests
    {
        [Fact]
        public void MissingConfigurationException()
        {
            var compareConfiguration = new CompareConfiguration();
            var comparer = compareConfiguration.CreateComparer();

            Assert.Throws<MissingConfigurationException>(() => comparer.Compare(Enumerable.Empty<Entities.Simple.EntityLevel0>(), Enumerable.Empty<Entities.Simple.EntityLevel0>()).ToArray());
        }
    }
}
