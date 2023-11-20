using BenchmarkDotNet.Attributes;
using EntityMerger.Configuration;

namespace EntityMerger.Benchmark;

public class LoadNoNavigation
{
    private Random Random { get; }
    private IReadOnlyCollection<NoNavigationEntity> ExistingEntities { get; set; } = null!;
    private IReadOnlyCollection<NoNavigationEntity> CalculatedEntities { get; set; } = null!;

    private IMerger NoHashtableNaiveComparerMerger { get; }
    private IMerger NoHastablePrecompileComparerMerger { get; }
    private IMerger HastableNaiveComparerMerger { get; }
    private IMerger HashtableMerger { get; }

    public LoadNoNavigation()
    {
        Random = new Random();

        var noHashtableNoPrecompiledComparerMergeConfiguration = new MergeConfiguration();
        noHashtableNoPrecompiledComparerMergeConfiguration
            .DisableHashtable()
            .Entity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference }, opt => opt.DisablePrecompiledEqualityComparer())
                .HasCalculatedValue(x => new { x.Penalty, x.Volume, x.Price }, opt => opt.DisablePrecompiledEqualityComparer())
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
        NoHashtableNaiveComparerMerger = noHashtableNoPrecompiledComparerMergeConfiguration.CreateMerger();

        var noHastablePrecompileComparerMergerConfiguration = new MergeConfiguration();
        noHastablePrecompileComparerMergerConfiguration
            .DisableHashtable()
            .Entity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference })
                .HasCalculatedValue(x => new { x.Penalty, x.Volume, x.Price })
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
        NoHastablePrecompileComparerMerger = noHastablePrecompileComparerMergerConfiguration.CreateMerger();

        var hastableNoPrecomileComparerMergerConfiguration = new MergeConfiguration();
        hastableNoPrecomileComparerMergerConfiguration
            .Entity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference }, opt => opt.DisablePrecompiledEqualityComparer())
                .HasCalculatedValue(x => new { x.Penalty, x.Volume, x.Price }, opt => opt.DisablePrecompiledEqualityComparer())
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
        HastableNaiveComparerMerger = hastableNoPrecomileComparerMergerConfiguration.CreateMerger();

        var hashtableMergerConfiguration = new MergeConfiguration();
        hashtableMergerConfiguration
            .Entity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference })
                .HasCalculatedValue(x => new { x.Penalty, x.Volume, x.Price })
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
        HashtableMerger = hashtableMergerConfiguration.CreateMerger();
    }

    [Params(10, 100, 1000)]
    public int N { get; set; }

    [Params(DataGenerationOptions.Identical, DataGenerationOptions.NoExisting, DataGenerationOptions.NoCalculated, DataGenerationOptions.Random)]
    public DataGenerationOptions Option { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        switch (Option)
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

    [Benchmark]
    public void NoHashtableNaiveComparerMerge()
    {
        var results = NoHashtableNaiveComparerMerger.Merge(ExistingEntities, CalculatedEntities).ToList();
    }

    [Benchmark]
    public void NoHastablePrecompileComparerMerge()
    {
        var results = NoHastablePrecompileComparerMerger.Merge(ExistingEntities, CalculatedEntities).ToList();
    }

    [Benchmark]
    public void HastableNaiveComparerMerge()
    {
        var results = HastableNaiveComparerMerger.Merge(ExistingEntities, CalculatedEntities).ToList();
    }

    [Benchmark]
    public void HashtableMerge()
    {
        var results = HashtableMerger.Merge(ExistingEntities, CalculatedEntities).ToList();
    }

    private void GenerateIdentical()
    {
        ExistingEntities = Enumerable.Range(0, N)
            .Select(x => new NoNavigationEntity
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today.AddDays(x),
                ContractReference = "REF",
                Price = x,
                Penalty = 2 * x,
                Volume = x
            }).ToArray();
        CalculatedEntities = Enumerable.Range(0, N)
            .Select(x => new NoNavigationEntity
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today.AddDays(x),
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
                Date = DateTime.Today.AddDays(x),
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
                Date = DateTime.Today.AddDays(x),
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
                Date = DateTime.Today.AddDays(x),
                ContractReference = $"REF{Random.Next() % 4}",
                Price = Random.Next() % 10,
                Penalty = 2 * (Random.Next() % 10),
                Volume = Random.Next() % 10
            }).ToArray();
        CalculatedEntities = Enumerable.Range(0, N)
            .Select(x => new NoNavigationEntity
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today.AddDays(x),
                ContractReference = $"REF{Random.Next() % 4}",
                Price = Random.Next() % 10,
                Penalty = 2 * (Random.Next() % 10),
                Volume = Random.Next() % 10
            }).ToArray();
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
