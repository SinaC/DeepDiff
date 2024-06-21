using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Arc4u;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.ActivationControl
{
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ActivationControlDpDetail : PersistEntity
    {
        // composite PK (PK from ActivationControlDetail + DeliveryPointEan)
        // PK from ActivationControlDetail
        public int ActivationControlId { get; set; }
        public DateTime StartsOn { get; set; }
        // PK
        public string DeliveryPointEan { get; set; }

        public string DeliveryPointName { get; set; }
        public Direction Direction { get; set; }
        public DeliveryPointType DeliveryPointType { get; set; }
        public decimal TotalEnergySupplied { get; set; }

        // one-to-many
        public List<ActivationControlDpTimestampDetail> TimestampDetails { get; set; }

        // FK to ActivationControlDetail
        public ActivationControlDetail ActivationControlDetail { get; set; }

        private string DebuggerDisplay => $"{DeliveryPointEan} {DeliveryPointName} DPT:{DeliveryPointType} TES:{TotalEnergySupplied} {ActivationControlId}-{StartsOn}";
    }
}
