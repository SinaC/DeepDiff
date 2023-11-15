using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EntityMerger.UnitTest.Entities.ActivationControl;

[DebuggerDisplay("{DebuggerDisplay, nq}")]
public class ActivationControlDetail : PersistEntity
{
    // composite PK (PK from ActivationControl + StartsOn)
    // PK from ActivationControl
    public Guid ActivationControlId { get; set; }
    // PK
    public DateTime StartsOn { get; set; }

    public decimal OfferedVolumeUp { get; set; }
    public decimal OfferedVolumeDown { get; set; }
    public decimal OfferedVolumeForRedispatchingUp { get; set; }
    public decimal OfferedVolumeForRedispatchingDown { get; set; }
    public decimal PermittedDeviationUp { get; set; }
    public decimal PermittedDeviationDown { get; set; }
    public decimal RampingRate { get; set; }
    public bool HasJump { get; set; }

    // one-to-many
    public List<ActivationControlTimestampDetail> TimestampDetails { get; set; }
    public List<ActivationControlDpDetail> DpDetails { get; set; }

    //
    public ActivationControl ActivationControl { get; set; }

    private string DebuggerDisplay => $"{StartsOn} OVU:{OfferedVolumeUp} OVD:{OfferedVolumeDown} OVRdU:{OfferedVolumeForRedispatchingUp} OVRdD:{OfferedVolumeForRedispatchingDown} PDeU:{PermittedDeviationUp} PDeD:{PermittedDeviationDown} RR:{RampingRate} J?:{HasJump} {ActivationControlId}";
}
