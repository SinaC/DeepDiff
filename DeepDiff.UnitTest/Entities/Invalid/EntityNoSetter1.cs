namespace DeepDiff.UnitTest.Entities.Invalid;

internal class EntityNoSetter1 : PersistEntity
{
    public Guid Id { get; set; }

    public DateTime Timestamp { get; set; }

    public decimal Power { get; } // read-only
    public decimal? Price { get; set; }

    public string Comment { get; set; } = null!;

    // debug property, will never participate in compare neither as key nor value
    public int Index { get; set; }
}
