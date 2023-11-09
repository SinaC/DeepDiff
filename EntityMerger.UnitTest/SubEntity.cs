using System;

namespace EntityMerger.UnitTest;

public class SubEntity : PersistEntity
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }

    public decimal Power { get; set; }
    public decimal? Price { get; set; }

    public string Comment { get; set; }
}
