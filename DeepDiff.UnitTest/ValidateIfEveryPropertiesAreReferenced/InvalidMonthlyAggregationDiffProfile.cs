using DeepDiff.Configuration;
using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Arc4u;
using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.MonthlyAggregation;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced
{
    public class InvalidMonthlyAggregationDiffProfile : DiffProfile
    {
        public InvalidMonthlyAggregationDiffProfile()
        {
            // SupplierEan is missing
            CreateConfiguration<MonthlyAggregation<MonthlyAggregationImputation>>()
                .AuditEntity()
                .HasKey(x => new { x.Month, x.ContractReference })
                .HasValues(x => x.SupplierName)
                .HasMany(x => x.Details);

            // Id, AuditedOn, AuditedBy are missing
            CreateConfiguration<MonthlyAggregationDetail<MonthlyAggregationImputation>>()
                .WithComparer<decimal>(new DecimalComparer(6))
                .WithComparer<decimal?>(new NullableDecimalComparer(6))
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Update))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .HasKey(x => x.ModuleKey)
                .HasValues(x => x.Status)
                .HasMany(x => x.MonthlyAggregationValues)
                .Ignore(x => new { x.MonthlyAggregationId, x.MonthlyAggregation });

            // ValueForValidated is missing
            CreateConfiguration<MonthlyAggregationImputation>()
                .AuditEntity()
                .HasKey(x => x.ImputationCategory)
                .HasValues(x => new { x.Value, x.EnergyRequested, x.EnergySupplied, x.EnergyDiscrepancy })
                .Ignore(x => new { x.MonthlyAggregationDetailId, x.MonthlyAggregationDetail });
        }
    }
}
