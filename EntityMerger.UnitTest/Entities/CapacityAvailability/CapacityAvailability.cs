using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EntityMerger.UnitTest.Entities.CapacityAvailability;

[DebuggerDisplay("{DebuggerDisplay, nq}")]
public class CapacityAvailability : CreateAuditEntity
{
    public DateTime Day { get; set; }
    public string CapacityMarketUnitId { get; set; }

    public bool IsEnergyContrained { get; set; }

    // one-to-many
    public List<CapacityAvailabilityDetail> CapacityAvailabilityDetails { get; set; }

    private string DebuggerDisplay => $"{CapacityMarketUnitId} {Day} {Id}";
}
