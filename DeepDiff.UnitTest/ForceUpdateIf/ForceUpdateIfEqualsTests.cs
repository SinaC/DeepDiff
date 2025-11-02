using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities;
using Xunit;

namespace DeepDiff.UnitTest.ForceUpdateIf
{
    public class ForceUpdateIfEqualsTests
    {
        [Fact]
        public void FromToBeCalculatedToCalculated_WithoutForceUpdate()
        {
            var deepDiff = CreateDeepDiffWithoutForceUpdate();

            var (existingEntity, newEntity) = GenerateEntities(FcrActivationControlStatus.ToBeCalculated, FcrActivationControlStatus.Calculated);
            var result = deepDiff.MergeSingle(existingEntity, newEntity);

            Assert.NotNull(result);
            Assert.Equal(FcrActivationControlStatus.ToBeCalculated, result.Status); // status has NOT been updated because no modification has been detected at FcrActivationControl level
        }

        [Fact]
        public void FromToBeCalculatedToCalculated_WithForceUpdate()
        {
            var deepDiff = CreateDeepDiffWithForceUpdateIf();

            var (existingEntity, newEntity) = GenerateEntities(FcrActivationControlStatus.ToBeCalculated, FcrActivationControlStatus.Calculated);
            var result = deepDiff.MergeSingle(existingEntity, newEntity);

            Assert.NotNull(result);
            Assert.Equal(FcrActivationControlStatus.Calculated, result.Status); // status modified because it was ToBeCalculated
        }

        [Fact]
        public void FromValidatedToCalculated_WithForceUpdate()
        {
            var deepDiff = CreateDeepDiffWithForceUpdateIf();

            var (existingEntity, newEntity) = GenerateEntities(FcrActivationControlStatus.Validated, FcrActivationControlStatus.Calculated);
            var result = deepDiff.MergeSingle(existingEntity, newEntity);

            Assert.NotNull(result);
            Assert.Equal(FcrActivationControlStatus.Validated, result.Status); // status has NOT been updated because it was not ToBeCalculated
        }

        private static IDeepDiff CreateDeepDiffWithoutForceUpdate()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<FcrActivationControl>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update).CopyValues(x => x.Status))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.Day, x.ContractReference })
                // no values
                .HasOne(x => x.FcrActivationControlDetail);
            diffConfiguration.ConfigureEntity<FcrActivationControlDetail>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .NoKey()
                .HasValues(x => new { x.Value1, x.Value2 });
            var deepDiff = diffConfiguration.CreateDeepDiff();
            return deepDiff;
        }

        private static IDeepDiff CreateDeepDiffWithForceUpdateIf()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.ConfigureEntity<FcrActivationControl>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update).CopyValues(x => x.Status))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => new { x.Day, x.ContractReference })
                // no values
                .HasOne(x => x.FcrActivationControlDetail)
                .ForceUpdateIf(cfg => cfg.Equals(x => x.Status, FcrActivationControlStatus.ToBeCalculated)); // force OnUpdate if status is ToBeCalculated
            diffConfiguration.ConfigureEntity<FcrActivationControlDetail>()
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .NoKey()
                .HasValues(x => new { x.Value1, x.Value2 });
            var deepDiff = diffConfiguration.CreateDeepDiff();
            return deepDiff;
        }

        private static (FcrActivationControl existingEntity, FcrActivationControl newEntity) GenerateEntities(FcrActivationControlStatus statusForExistingEntity, FcrActivationControlStatus statusForNewEntity)
        {
            var existingEntity = new FcrActivationControl
            {
                Day = Date.Today,
                ContractReference = "CREF",

                Status = statusForExistingEntity,

                FcrActivationControlDetail = null!
            };

            var newEntity = new FcrActivationControl
            {
                Day = Date.Today,
                ContractReference = "CREF",

                Status = statusForNewEntity,

                FcrActivationControlDetail = new FcrActivationControlDetail
                {
                    Value1 = 1,
                    Value2 = 2
                }
            };

            return (existingEntity, newEntity);
        }

        internal enum FcrActivationControlStatus
        {
            ToBeCalculated = 1,
            Calculated = 2,
            Validated = 3
        }

        internal class FcrActivationControl : CreateAuditEntity<int>
        {
            public Date Day { get; set; }
            public string ContractReference { get; set; } = null!;

            public FcrActivationControlStatus Status { get; set; }

            // one-to-zero/one
            public FcrActivationControlDetail FcrActivationControlDetail { get; set; } = null!;
        }

        internal class FcrActivationControlDetail : PersistEntity
        {
            public decimal Value1 { get; set; }
            public decimal Value2 { get; set; }

            // FK
            public int FcrActivationControlId { get; set; }
            public FcrActivationControl FcrActivationControl { get; set; } = null!;
        }
    }
}
