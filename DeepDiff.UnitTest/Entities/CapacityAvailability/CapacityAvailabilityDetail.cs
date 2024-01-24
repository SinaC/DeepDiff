using System;
using System.Diagnostics;

namespace DeepDiff.UnitTest.Entities.CapacityAvailability;

[DebuggerDisplay("{DebuggerDisplay, nq}")]
public class CapacityAvailabilityDetail : UpdateAuditEntity<Guid>
{
    public DateTime StartsOn { get; set; }

    public decimal ObligatedVolume { get; set; }
    public decimal AvailableVolume { get; set; }
    public decimal MissingVolume { get; set; }

    public CapacityAvailabilityStatus Status { get; set; }
    public string Comment { get; set; }

    // FK to CapacityAvailability
    public Guid CapacityAvailabilityId { get; set; }
    public CapacityAvailability CapacityAvailability { get; set; }

    private string DebuggerDisplay => $"{StartsOn} OV:{ObligatedVolume} AV:{AvailableVolume} MV:{MissingVolume} {Status} {Id}";
}
