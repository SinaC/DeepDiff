using BenchmarkDotNet.Attributes;
using DeepDiff.Benchmark.Entities;
using DeepDiff.Configuration;

namespace DeepDiff.Benchmark;

public class LoadNoNavigation
{
    private Random Random { get; }
    private IReadOnlyCollection<NoNavigationEntity> ExistingEntities { get; set; } = null!;
    private IReadOnlyCollection<NoNavigationEntity> NewEntities { get; set; } = null!;

    private IDeepDiff NoHashtableNaiveComparerDeepDiff { get; }
    private IDeepDiff NoHashtablePrecompiledComparerDeepDiff { get; }
    private IDeepDiff HashtableNaiveComparerDeepDiff { get; }
    private IDeepDiff HashtablePrecompiledComparerDeepDiff { get; }

    public LoadNoNavigation()
    {
        Random = new Random();

        var noHashtableNaiveComparerDiffConfiguration = new DeepDiffConfiguration();
        noHashtableNaiveComparerDiffConfiguration
            .ConfigureEntity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference })
                .HasValues(x => new { x.Penalty, x.Volume, x.Price })
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));
        NoHashtableNaiveComparerDeepDiff = noHashtableNaiveComparerDiffConfiguration.CreateDeepDiff();

        var noHastablePrecompiledComparerDiffConfiguration = new DeepDiffConfiguration();
        noHastablePrecompiledComparerDiffConfiguration
            .ConfigureEntity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference })
                .HasValues(x => new { x.Penalty, x.Volume, x.Price })
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));
        NoHashtablePrecompiledComparerDeepDiff = noHastablePrecompiledComparerDiffConfiguration.CreateDeepDiff();

        var hastableNaiveComparerDiffConfiguration = new DeepDiffConfiguration();
        hastableNaiveComparerDiffConfiguration
            .ConfigureEntity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference })
                .HasValues(x => new { x.Penalty, x.Volume, x.Price })
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));
        HashtableNaiveComparerDeepDiff = hastableNaiveComparerDiffConfiguration.CreateDeepDiff();

        var hashtableDiffConfiguration = new DeepDiffConfiguration();
        hashtableDiffConfiguration
            .ConfigureEntity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference })
                .HasValues(x => new { x.Penalty, x.Volume, x.Price })
                .OnInsert(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Insert))
                .OnDelete(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete))
                .OnUpdate(cfg => cfg.SetValue(x => x.PersistChange, PersistChange.Delete));
        HashtablePrecompiledComparerDeepDiff = hashtableDiffConfiguration.CreateDeepDiff();
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
        var results = NoHashtableNaiveComparerDeepDiff.MergeMany(ExistingEntities, NewEntities, cfg => cfg.UseHashtable(false).SetEqualityComparer(EqualityComparers.Naive)).ToList();
    }

    [Benchmark]
    public void NoHastablePrecompileComparerDiff()
    {
        var results = NoHashtablePrecompiledComparerDeepDiff.MergeMany(ExistingEntities, NewEntities, cfg => cfg.UseHashtable(false).SetEqualityComparer(EqualityComparers.Precompiled)).ToList();
    }

    [Benchmark]
    public void HastableNaiveComparerDiff()
    {
        var results = HashtableNaiveComparerDeepDiff.MergeMany(ExistingEntities, NewEntities, cfg => cfg.SetEqualityComparer(EqualityComparers.Naive)).ToList();
    }

    [Benchmark]
    public void HashtablePrecompileComparerDiff()
    {
        var results = HashtablePrecompiledComparerDeepDiff.MergeMany(ExistingEntities, NewEntities, cfg => cfg.SetEqualityComparer(EqualityComparers.Precompiled)).ToList();
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
