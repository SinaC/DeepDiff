using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class DuplicatePropertyConfigurationExceptionTests
    {
        [Fact]
        public void OnInsert_SetValue_DuplicateProperty()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert).SetValue(x => x.PersistChange, PersistChange.Update))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.Penalty);

            var exception = Assert.Throws<DuplicatePropertyConfigurationException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Single(exception.DuplicatePropertyNames);
            Assert.Equal(nameof(Entities.Simple.EntityLevel0.PersistChange), exception.DuplicatePropertyNames.Single());
        }

        [Fact]
        public void OnUpdate_SetValue_DuplicateProperty()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert).SetValue(x => x.PersistChange, PersistChange.Update))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.Penalty);

            var exception = Assert.Throws<DuplicatePropertyConfigurationException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Single(exception.DuplicatePropertyNames);
            Assert.Equal(nameof(Entities.Simple.EntityLevel0.PersistChange), exception.DuplicatePropertyNames.Single());
        }

        [Fact]
        public void OnDelete_SetValue_DuplicateProperty()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert).SetValue(x => x.PersistChange, PersistChange.Update))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.Penalty);

            var exception = Assert.Throws<DuplicatePropertyConfigurationException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Single(exception.DuplicatePropertyNames);
            Assert.Equal(nameof(Entities.Simple.EntityLevel0.PersistChange), exception.DuplicatePropertyNames.Single());
        }
    }
}
