using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities;
using System;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class AlreadyDefinedPropertyExceptionTests
    {
        [Fact]
        public void Values_AlsoInKey()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.StartsOn);

            var exception = Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Single(exception.AlreadyDefinedPropertyNames);
            Assert.Contains(nameof(Entities.Simple.EntityLevel0.StartsOn), exception.AlreadyDefinedPropertyNames);
        }

        [Fact]
        public void CopyValues_AlsoInKey()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .OnUpdate(cfg => cfg.CopyValues(x => x.StartsOn));

            var exception = Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Single(exception.AlreadyDefinedPropertyNames);
            Assert.Contains(nameof(Entities.Simple.EntityLevel0.StartsOn), exception.AlreadyDefinedPropertyNames);
        }

        [Fact]
        public void CopyValues_AlsoInValues()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => new { x.Penalty })
                .OnUpdate(cfg => cfg.CopyValues(x => x.Penalty));

            var exception = Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Single(exception.AlreadyDefinedPropertyNames);
            Assert.Contains(nameof(Entities.Simple.EntityLevel0.Penalty), exception.AlreadyDefinedPropertyNames);
        }

        [Fact]
        public void CopyValues_AlsoInSetValue()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .OnUpdate(cfg => cfg.CopyValues(x => x.PersistChange));

            var exception = Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Single(exception.AlreadyDefinedPropertyNames);
            Assert.Contains(nameof(Entities.Simple.EntityLevel0.PersistChange), exception.AlreadyDefinedPropertyNames);
        }

        [Fact]
        public void Update_SetValue_AlsoInKey()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnUpdate(cfg => cfg.SetValue(x => x.StartsOn, DateTime.UtcNow))
                .HasKey(x => new { x.StartsOn, x.Direction });

            var exception = Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Single(exception.AlreadyDefinedPropertyNames);
            Assert.Contains(nameof(Entities.Simple.EntityLevel0.StartsOn), exception.AlreadyDefinedPropertyNames);
        }

        [Fact]
        public void Update_SetValue_AlsoInValues()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnUpdate(cfg => cfg.SetValue(x => x.Penalty, -999m))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.Penalty);

            var exception = Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Single(exception.AlreadyDefinedPropertyNames);
            Assert.Contains(nameof(Entities.Simple.EntityLevel0.Penalty), exception.AlreadyDefinedPropertyNames);
        }

        [Fact]
        public void Insert_SetValue_AlsoInKey()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.StartsOn, DateTime.UtcNow))
                .HasKey(x => new { x.StartsOn, x.Direction });

            var exception = Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Single(exception.AlreadyDefinedPropertyNames);
            Assert.Contains(nameof(Entities.Simple.EntityLevel0.StartsOn), exception.AlreadyDefinedPropertyNames);
        }

        [Fact]
        public void Insert_SetValue_AlsoInValues()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnInsert(cfg => cfg.SetValue(x => x.Penalty, -999m))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.Penalty);

            var exception = Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Single(exception.AlreadyDefinedPropertyNames);
            Assert.Contains(nameof(Entities.Simple.EntityLevel0.Penalty), exception.AlreadyDefinedPropertyNames);
        }

        [Fact]
        public void Delete_SetValue_AlsoInKey()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnDelete(cfg => cfg.SetValue(x => x.StartsOn, DateTime.UtcNow))
                .HasKey(x => new { x.StartsOn, x.Direction });

            var exception = Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Single(exception.AlreadyDefinedPropertyNames);
            Assert.Contains(nameof(Entities.Simple.EntityLevel0.StartsOn), exception.AlreadyDefinedPropertyNames);
        }

        [Fact]
        public void Delete_SetValue_AlsoInValues()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<Entities.Simple.EntityLevel0>()
                .OnDelete(cfg => cfg.SetValue(x => x.Penalty, -999m))
                .HasKey(x => new { x.StartsOn, x.Direction })
                .HasValues(x => x.Penalty);

            var exception = Assert.Throws<AlreadyDefinedPropertyException>(() => diffConfiguration.CreateDeepDiff());
            Assert.Single(exception.AlreadyDefinedPropertyNames);
            Assert.Contains(nameof(Entities.Simple.EntityLevel0.Penalty), exception.AlreadyDefinedPropertyNames);
        }
    }
}
