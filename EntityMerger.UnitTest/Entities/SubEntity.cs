using System;

namespace EntityMerger.UnitTest.Entities;

internal class SubEntity : PersistEntity
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }

    public decimal Power { get; set; }
    public decimal? Price { get; set; }

    public string Comment { get; set; } = null!;

    // debug property, will never participate in merge neither as key nor value
    public int Index { get; set; }
}
