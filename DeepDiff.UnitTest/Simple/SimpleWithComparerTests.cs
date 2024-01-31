using DeepDiff.Configuration;
using DeepDiff.Operations;
using DeepDiff.UnitTest.Entities;
using DeepDiff.UnitTest.Entities.Simple;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Simple
{
    public class SimpleWithComparerTests
    {
        [Fact]
        public void ComparersOnType_Modifications()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel2>()
                .WithComparer(new DecimalComparer(3))
                .WithComparer(new NullableDecimalComparer(5))
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.Value1, x.Value2 });
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var existingEntity = new EntityLevel2
            {
                DeliveryPointEan = "DPEAN",

                Value1 = 1.123456m,
                Value2 = 1.123456m
            };

            var newEntity = new EntityLevel2
            {
                DeliveryPointEan = "DPEAN",

                Value1 = 1.124m, // 3rd decimal is different -> will be considered different
                Value2 = 1.12346m // 5th decimal is different -> will be considered different
            };

            var diff = deepDiff.DiffSingle(existingEntity, newEntity);
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.NotNull(result);
            Assert.Same(result, existingEntity);
            Assert.Equal(PersistChange.Update, result.PersistChange);
            Assert.Equal(1.124m, result.Value1); // copied
            Assert.Equal(1.12346m, result.Value2); // copied
            Assert.Single(diff.Operations);
            Assert.Single(diff.Operations.OfType<UpdateDiffOperation>());
            Assert.Equal(2, diff.Operations.OfType<UpdateDiffOperation>().Single().UpdatedProperties.Count);
        }

        [Fact]
        public void ComparersOnType_NoModifications()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel2>()
                .WithComparer(new DecimalComparer(3))
                .WithComparer(new NullableDecimalComparer(5))
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.Value1, x.Value2 });
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var existingEntity = new EntityLevel2
            {
                DeliveryPointEan = "DPEAN",

                Value1 = 1.123456m,
                Value2 = 1.123456m
            };

            var newEntity = new EntityLevel2
            {
                DeliveryPointEan = "DPEAN",

                Value1 = 1.1239m, // 4th decimal is different but decimal3 is used -> will be considered same
                Value2 = 1.123459m // 6th decimal is different but decimal5 is used -> will be considered same
            };

            var diff = deepDiff.DiffSingle(existingEntity, newEntity);
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.Null(result);
        }

        [Fact]
        public void ComparersOnType_ComparerOnProperty_Modifications()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel2>()
                .WithComparer(new DecimalComparer(3))
                .WithComparer(new NullableDecimalComparer(5))
                .WithComparer(x => x.Value2, new NullableDecimalComparer(6))
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.DeliveryPointEan)
                .HasValues(x => new { x.Value1, x.Value2 });
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var existingEntity = new EntityLevel2
            {
                DeliveryPointEan = "DPEAN",

                Value1 = 1.123456m,
                Value2 = 1.123456m
            };

            var newEntity = new EntityLevel2
            {
                DeliveryPointEan = "DPEAN",

                Value1 = 1.124m, // 3rd decimal is different -> will be considered different
                Value2 = 1.1234567m // 7th decimal is different -> will be considered same
            };

            var diff = deepDiff.DiffSingle(existingEntity, newEntity, cfg => cfg.DisablePrecompiledEqualityComparer());
            var result = diff.Entity;
            var operations = diff.Operations;

            Assert.NotNull(result);
            Assert.Same(result, existingEntity);
            Assert.Equal(PersistChange.Update, result.PersistChange);
            Assert.Equal(1.124m, result.Value1); // copied
            Assert.Equal(1.123456m, result.Value2); // not copied
            Assert.Single(diff.Operations);
            Assert.Single(diff.Operations.OfType<UpdateDiffOperation>());
            Assert.Single(diff.Operations.OfType<UpdateDiffOperation>().Single().UpdatedProperties);
        }
    }
}
