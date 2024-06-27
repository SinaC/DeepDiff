using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Seas;
using System;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.MonthlyAggregation
{
    public abstract class MonthlyAggregationValues : AuditEntity
    {
        public decimal Value { get; set; } // can be Remuneration or Penalty depending on ImputationCategory (whatever the related Status is)
        public decimal ValueForValidated { get; set; } // can be Remuneration or Penalty depending on ImputationCategory (only for validated related Status)

        // FK
        public Guid MonthlyAggregationDetailId { get; set; }
        //public MonthlyAggregationDetail MonthlyAggregationDetail { get; set; } // Removed in base class, added in derived class for navigation
    }
}
