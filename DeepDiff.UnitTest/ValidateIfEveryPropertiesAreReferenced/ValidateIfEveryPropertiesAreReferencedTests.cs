using DeepDiff.Configuration;
using Xunit;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced
{
    public class ValidateIfEveryPropertiesAreReferencedTests
    {
        [Fact]
        public void Validate()
        {
            var config = new DeepDiffConfiguration();
            config.AddProfile(new CapacityAvailabilityDiffProfile());
            config.AddProfile(new ActivationControlDiffProfile());
            config.AddProfile(new ActivationRemunerationDiffProfile());

            config.ValidateConfiguration();
            config.ValidateIfEveryPropertiesAreReferenced();
        }
    }
}
