namespace DeepDiff.Benchmark.Entities;

internal class NavigationEntityLevel0
{
    public Guid Id { get; set; }

    public DateTime StartsOn { get; set; }
    public Direction Direction { get; set; }

    public decimal RequestedPower { get; set; }
    public decimal? Penalty { get; set; }

    public string AdditionalValueToCopy { get; set; } = null!;
    public string Comment { get; set; } = null!;

    // one-to-one
    public NavigationEntityLevel1 SubEntity { get; set; } = null!;

    // one-to-many
    public List<NavigationEntityLevel1> SubEntities { get; set; } = null!;

    //
    public PersistChange PersistChange { get; set; }
}
