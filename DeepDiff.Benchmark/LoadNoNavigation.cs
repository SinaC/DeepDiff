using BenchmarkDotNet.Attributes;
using DeepDiff.Configuration;

namespace DeepDiff.Benchmark;

public class LoadNoNavigation
{
    private Random Random { get; }
    private IReadOnlyCollection<NoNavigationEntity> ExistingEntities { get; set; } = null!;
    private IReadOnlyCollection<NoNavigationEntity> NewEntities { get; set; } = null!;

    private IDeepDiff NoHashtableNaiveComparerDeepDiff { get; }
    private IDeepDiff NoHastablePrecompileComparerDeepDiff { get; }
    private IDeepDiff HastableNaiveComparerDeepDiff { get; }
    private IDeepDiff HashtablePrecompileComparerDeepDiff { get; }

    public LoadNoNavigation()
    {
        Random = new Random();

        var noHashtableNoPrecompiledComparerDiffConfiguration = new DiffConfiguration();
        noHashtableNoPrecompiledComparerDiffConfiguration
            .DisableHashtable()
            .Entity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference }, opt => opt.DisablePrecompiledEqualityComparer())
                .HasValues(x => new { x.Penalty, x.Volume, x.Price }, opt => opt.DisablePrecompiledEqualityComparer())
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
        NoHashtableNaiveComparerDeepDiff = noHashtableNoPrecompiledComparerDiffConfiguration.CreateDeepDiff();

        var noHastablePrecompileComparerDiffConfiguration = new DiffConfiguration();
        noHastablePrecompileComparerDiffConfiguration
            .DisableHashtable()
            .Entity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference })
                .HasValues(x => new { x.Penalty, x.Volume, x.Price })
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
        NoHastablePrecompileComparerDeepDiff = noHastablePrecompileComparerDiffConfiguration.CreateDeepDiff();

        var hastableNoPrecomileComparerDiffConfiguration = new DiffConfiguration();
        hastableNoPrecomileComparerDiffConfiguration
            .Entity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference }, opt => opt.DisablePrecompiledEqualityComparer())
                .HasValues(x => new { x.Penalty, x.Volume, x.Price }, opt => opt.DisablePrecompiledEqualityComparer())
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
        HastableNaiveComparerDeepDiff = hastableNoPrecomileComparerDiffConfiguration.CreateDeepDiff();

        var hashtableDiffConfiguration = new DiffConfiguration();
        hashtableDiffConfiguration
            .Entity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference })
                .HasValues(x => new { x.Penalty, x.Volume, x.Price })
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
        HashtablePrecompileComparerDeepDiff = hashtableDiffConfiguration.CreateDeepDiff();
    }

    [Params(10, 100, 1000)]
    public int N { get; set; }

    [Params(DataGenerationOptions.Identical, DataGenerationOptions.NoExisting, DataGenerationOptions.NoNew, DataGenerationOptions.Random)]
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
            case DataGenerationOptions.NoNew:
                GenerateNoNew();
                break;
            case DataGenerationOptions.Random:
                GenerateRandom();
                break;
        }
    }

    [Benchmark]
    public void NoHashtableNaiveComparerDiff()
    {
        var results = NoHashtableNaiveComparerDeepDiff.DiffMany(ExistingEntities, NewEntities).ToList();
    }

    [Benchmark]
    public void NoHastablePrecompileComparerDiff()
    {
        var results = NoHastablePrecompileComparerDeepDiff.DiffMany(ExistingEntities, NewEntities).ToList();
    }

    [Benchmark]
    public void HastableNaiveComparerDiff()
    {
        var results = HastableNaiveComparerDeepDiff.DiffMany(ExistingEntities, NewEntities).ToList();
    }

    [Benchmark]
    public void HashtablePrecompileComparerDiff()
    {
        var results = HashtablePrecompileComparerDeepDiff.DiffMany(ExistingEntities, NewEntities).ToList();
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
        NewEntities = Enumerable.Range(0, N)
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
        NewEntities = Enumerable.Range(0, N)
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

    private void GenerateNoNew()
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
        NewEntities = Array.Empty<NoNavigationEntity>();
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
        NewEntities = Enumerable.Range(0, N)
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
        NoNew,
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

    //
    public PersistChange PersistChange { get; set; }
}

public enum PersistChange
{
    None,
    Insert,
    Update,
    Delete
}
