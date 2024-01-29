using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class DuplicatePropertySpecificComparerConfigurationExceptionTests
    {
        [Fact]
        public void DuplicatePropertySpecificConverter()
        {
            var diffConfiguration = new DiffConfiguration();
            var diffEntityConfiguration = diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.RequestedPower)
                .WithComparer(x => x.RequestedPower, new DecimalComparer(6));

            Assert.Throws<DuplicatePropertySpecificComparerConfigurationException>(() => diffEntityConfiguration.WithComparer(x => x.RequestedPower, new DecimalComparer(6)));
        }
    }
}
