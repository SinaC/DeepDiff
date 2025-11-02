namespace DeepDiff.POC.Benchmark.Entities;

internal class NavigationEntityLevel2
{
    public Guid Id { get; set; }

    public string DeliveryPointEan { get; set; } = null!;

    public decimal Value1 { get; set; }
    public decimal? Value2 { get; set; }

    //
    public PersistChange PersistChange { get; set; }
}
