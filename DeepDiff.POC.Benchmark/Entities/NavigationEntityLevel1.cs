namespace DeepDiff.POC.Benchmark.Entities;

internal class NavigationEntityLevel1
{
    public Guid Id { get; set; }

    public DateTime Timestamp { get; set; }

    public decimal Power { get; set; }
    public decimal? Price { get; set; }

    public string Comment { get; set; } = null!;

    // one-to-one
    public NavigationEntityLevel2 SubEntity { get; set; } = null!;

    // one-to-many
    public List<NavigationEntityLevel2> SubEntities { get; set; } = null!;

    //
    public PersistChange PersistChange { get; set; }
}
