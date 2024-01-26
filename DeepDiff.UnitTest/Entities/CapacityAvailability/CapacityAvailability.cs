using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DeepDiff.UnitTest.Entities.CapacityAvailability;

[DebuggerDisplay("{DebuggerDisplay, nq}")]
public class CapacityAvailability : CreateAuditEntity<Guid>
{
    public DateTime Day { get; set; }
    public string CapacityMarketUnitId { get; set; } = null!;

    public bool IsEnergyContrained { get; set; }

    // one-to-many
    public List<CapacityAvailabilityDetail> CapacityAvailabilityDetails { get; set; } = null!;

    private string DebuggerDisplay => $"{CapacityMarketUnitId} {Day} {Id}";
}
