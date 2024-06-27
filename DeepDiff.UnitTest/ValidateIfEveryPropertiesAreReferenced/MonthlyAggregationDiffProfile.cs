using DeepDiff.Configuration;
using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.MonthlyAggregation;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced
{
    public class MonthlyAggregationDiffProfile : DiffProfile
    {
        public MonthlyAggregationDiffProfile()
        {
            CreateConfiguration<MonthlyAggregation<MonthlyAggregationImputation>>()
                .AuditEntity()
                .HasKey(x => new { x.Month, x.ContractReference, x.SupplierEan })
                .HasValues(x => x.SupplierName)
                .HasMany(x => x.Details);

            CreateConfiguration<MonthlyAggregationDetail<MonthlyAggregationImputation>>()
                .AuditEntity()
                .HasKey(x => x.ModuleKey)
                .HasValues(x => x.Status)
                .HasMany(x => x.MonthlyAggregationValues)
                .Ignore(x => new { x.MonthlyAggregationId, x.MonthlyAggregation });

            CreateConfiguration<MonthlyAggregationImputation>()
                .AuditEntity()
                .HasKey(x => x.ImputationCategory)
                .HasValues(x => new { x.Value, x.ValueForValidated, x.EnergyRequested, x.EnergySupplied, x.EnergyDiscrepancy })
                .Ignore(x => new { x.MonthlyAggregationDetailId, x.MonthlyAggregationDetail });
        }
    }
}
