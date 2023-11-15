using System;
using System.Diagnostics;

namespace EntityMerger.UnitTest.Entities.ActivationControl;

[DebuggerDisplay("{DebuggerDisplay, nq}")]
public class ActivationControlDpTimestampDetail : PersistEntity
{
    // composite PK (PK from ActivationControlDpDetail + Timestamp)
    // PK from ActivationControlDpDetail
    public Guid ActivationControlId { get; set; }
    public DateTime StartsOn { get; set; }
    public string DeliveryPointEan { get; set; }
    // PK
    public DateTime Timestamp { get; set; }

    public decimal PowerMeasured { get; set; }
    public decimal PowerBaseline { get; set; }
    public decimal FcrCorrection { get; set; }
    public decimal EnergySupplied { get; set; }

    //
    public ActivationControlDpDetail ActivationControlDpDetail { get; set; }

    private string DebuggerDisplay => $"{Timestamp} PM:{PowerMeasured} PB:{PowerBaseline} FC:{FcrCorrection} ES:{EnergySupplied} {ActivationControlId}-{StartsOn}-{DeliveryPointEan}";
}
