using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Arc4u;
using DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.Seas;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DeepDiff.UnitTest.ValidateIfEveryPropertiesAreReferenced.Entities.ActivationRemuneration
{
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ActivationRemunerationDetail : PersistEntity, IAuditEntity<string, DateTime>
    {
        // composite PK (PK from ActivationRemuneration + StartsOn)
        // PK from ActivationRemuneration
        public int ActivationRemunerationId { get; set; }
        // PK
        public DateTime StartsOn { get; set; }

        public bool IsProrata { get; set; }
        public decimal EnergyRequested { get; set; }
        public decimal EnergyRequestedSendToSupplier { get; set; }
        public decimal EnergyRequestedRatio { get; set; }
        public bool IsEnergyRequestedRatioIncoherent { get; set; }

        public ActivationRemunerationStatus Status { get; set; }
        public string InternalComment { get; set; }
        public string TsoComment { get; set; }
        public SupplierStatus SupplierStatus { get; set; }
        public string SupplierComment { get; set; }

        // one-to-many
        public List<ActivationRemunerationDirectionDetail> ActivationRemunerationDirectionDetails { get; set; } // one by direction

        // FK to ActivationRemuneration
        public ActivationRemuneration ActivationRemuneration { get; set; }

        // technical fields
        public string AuditedBy { get; set; }
        public DateTime AuditedOn { get; set; }

        //
        private string DebuggerDisplay => $"{StartsOn} PR?:{IsProrata} ER:{EnergyRequested} ERSTS:{EnergyRequestedSendToSupplier} ERR:{EnergyRequestedRatio} ERRI?:{IsEnergyRequestedRatioIncoherent} {ActivationRemunerationId}";
    }
}
