using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using EntityMerger.EntityMerger;

namespace EntityMerger.Benchmark;

[SimpleJob(RuntimeMoniker.Net60)]
public class LoadNoNavigation
{
    private Random Random { get; }
    private IReadOnlyCollection<NoNavigationEntity> ExistingEntities { get; set; } = null!;
    private IReadOnlyCollection<NoNavigationEntity> CalculatedEntities { get; set; } = null!;

    private IMerger Merger {get; }

    public LoadNoNavigation()
    {
        Random = new Random();

        var mergeConfiguration = new MergeConfiguration();
        mergeConfiguration
            .Entity<NoNavigationEntity>()
            .HasKey(x => new {x.Date, x.ContractReference})
            .HasCalculatedValue(x => new {x.Penalty, x.Volume, x.Price})
            .OnInsert(x => x.PersistChange, PersistChange.Insert)
            .OnUpdate(x => x.PersistChange, PersistChange.Update)
            .OnDelete(x => x.PersistChange, PersistChange.Delete);
        Merger = mergeConfiguration.CreateMerger();
    }

    [Params(10, 1000, 1000000)]
    public int N { get; set; }

    [Params(DataGenerationOptions.Identical, DataGenerationOptions.NoExisting, DataGenerationOptions.NoCalculated, DataGenerationOptions.Random)]
    public DataGenerationOptions Opt { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        switch (Opt)
        {
            case DataGenerationOptions.Identical:
                GenerateIdentical();
                break;
            case DataGenerationOptions.NoExisting:
                GenerateNoExisting();
                break;
            case DataGenerationOptions.NoCalculated:
                GenerateNoCalculated();
                break;
            case DataGenerationOptions.Random:
                GenerateRandom();
                break;
        }
    }

    private void GenerateIdentical()
    {
        ExistingEntities = Enumerable.Range(0, N)
            .Select(x => new NoNavigationEntity
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today,
                ContractReference = "REF",
                Price = x,
                Penalty = 2 * x,
                Volume = x
            }).ToArray();
        CalculatedEntities = Enumerable.Range(0, N)
            .Select(x => new NoNavigationEntity
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today,
                ContractReference = "REF",
                Price = x,
                Penalty = 2 * x,
                Volume = x
            }).ToArray();
    }

    private void GenerateNoExisting()
    {
        ExistingEntities = Array.Empty<NoNavigationEntity>();
        CalculatedEntities = Enumerable.Range(0, N)
            .Select(x => new NoNavigationEntity
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today,
                ContractReference = "REF",
                Price = x,
                Penalty = 2 * x,
                Volume = x
            }).ToArray();
    }

    private void GenerateNoCalculated()
    {
        ExistingEntities = Enumerable.Range(0, N)
            .Select(x => new NoNavigationEntity
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today,
                ContractReference = "REF",
                Price = x,
                Penalty = 2 * x,
                Volume = x
            }).ToArray();
        CalculatedEntities = Array.Empty<NoNavigationEntity>();
    }

    private void GenerateRandom()
    {
        ExistingEntities = Enumerable.Range(0, N)
            .Select(x => new NoNavigationEntity
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today,
                ContractReference = $"REF{Random.Next() % 10}",
                Price = Random.Next() % 10,
                Penalty = 2 * (Random.Next() % 10),
                Volume = Random.Next() % 10
            }).ToArray();
        CalculatedEntities = Enumerable.Range(0, N)
            .Select(x => new NoNavigationEntity
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today,
                ContractReference = $"REF{Random.Next() % 10}",
                Price = Random.Next() % 10,
                Penalty = 2 * (Random.Next() % 10),
                Volume = Random.Next() % 10
            }).ToArray();
    }

    [Benchmark]
    public void Merge()
    {
        Merger.Merge(ExistingEntities, CalculatedEntities);
    }

    public enum DataGenerationOptions
    {
        Identical,
        NoExisting,
        NoCalculated,
        Random
    }
}

public class NoNavigationEntity
{
    // DB key
    public Guid Id { get; set; }

    // Business key
    public DateTime Date { get; set; }
    public string ContractReference { get; set; } = null!;

    // Calculated values
    public decimal Penalty { get; set; }
    public decimal Volume { get; set; }
    public decimal Price { get; set; }

    public PersistChange PersistChange { get; set; }
}

public enum PersistChange
{
    None,
    Insert,
    Update,
    Delete
}
