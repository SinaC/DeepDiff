using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class AlreadyDefinedPropertyExceptionTests
    {
        [Fact]
        public void Values_AlsoInKey()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.StartsOn);

            Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
        }

        [Fact]
        public void AdditionalValuesToCopy_AlsoInKey()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .OnUpdate(cfg => cfg.CopyValues(x => x.StartsOn));

            Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
        }

        [Fact]
        public void AdditionalValuesToCopy_AlsoInValues()
        {
            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.Entity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => x.Penalty));

            Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
        }
    }
}
