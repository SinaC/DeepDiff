using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Inheritance.Entities.Abstract;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class AbstractEntityConfigurationExceptionTests
    {
        [Fact]
        public void AbstractEntityConfigurationException()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            Assert.Throws<AbstractEntityConfigurationException>(() => diffConfiguration.Entity<SubEntityBase>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.Key));
        }
    }
}
