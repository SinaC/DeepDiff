using BenchmarkDotNet.Attributes;
using EntityComparer.Configuration;

namespace EntityComparer.Benchmark;

public class LoadNoNavigation
{
    private Random Random { get; }
    private IReadOnlyCollection<NoNavigationEntity> ExistingEntities { get; set; } = null!;
    private IReadOnlyCollection<NoNavigationEntity> NewEntities { get; set; } = null!;

    private IEntityComparer NoHashtableNaiveComparerComparer { get; }
    private IEntityComparer NoHastablePrecompileComparerComparer { get; }
    private IEntityComparer HastableNaiveComparerComparer { get; }
    private IEntityComparer HashtablePrecompileComparerComparer { get; }

    public LoadNoNavigation()
    {
        Random = new Random();

        var noHashtableNoPrecompiledComparerCompareConfiguration = new CompareConfiguration();
        noHashtableNoPrecompiledComparerCompareConfiguration
            .DisableHashtable()
            .Entity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference }, opt => opt.DisablePrecompiledEqualityComparer())
                .HasValues(x => new { x.Penalty, x.Volume, x.Price }, opt => opt.DisablePrecompiledEqualityComparer())
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
        NoHashtableNaiveComparerComparer = noHashtableNoPrecompiledComparerCompareConfiguration.CreateComparer();

        var noHastablePrecompileComparerComparerConfiguration = new CompareConfiguration();
        noHastablePrecompileComparerComparerConfiguration
            .DisableHashtable()
            .Entity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference })
                .HasValues(x => new { x.Penalty, x.Volume, x.Price })
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
        NoHastablePrecompileComparerComparer = noHastablePrecompileComparerComparerConfiguration.CreateComparer();

        var hastableNoPrecomileComparerComparerConfiguration = new CompareConfiguration();
        hastableNoPrecomileComparerComparerConfiguration
            .Entity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference }, opt => opt.DisablePrecompiledEqualityComparer())
                .HasValues(x => new { x.Penalty, x.Volume, x.Price }, opt => opt.DisablePrecompiledEqualityComparer())
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
        HastableNaiveComparerComparer = hastableNoPrecomileComparerComparerConfiguration.CreateComparer();

        var hashtableComparerConfiguration = new CompareConfiguration();
        hashtableComparerConfiguration
            .Entity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference })
                .HasValues(x => new { x.Penalty, x.Volume, x.Price })
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
        HashtablePrecompileComparerComparer = hashtableComparerConfiguration.CreateComparer();
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
    public void NoHashtableNaiveComparerCompare()
    {
        var results = NoHashtableNaiveComparerComparer.Compare(ExistingEntities, NewEntities).ToList();
    }

    [Benchmark]
    public void NoHastablePrecompileComparerCompare()
    {
        var results = NoHastablePrecompileComparerComparer.Compare(ExistingEntities, NewEntities).ToList();
    }

    [Benchmark]
    public void HastableNaiveComparerCompare()
    {
        var results = HastableNaiveComparerComparer.Compare(ExistingEntities, NewEntities).ToList();
    }

    [Benchmark]
    public void HashtablePrecompileComparerCompare()
    {
        var results = HashtablePrecompileComparerComparer.Compare(ExistingEntities, NewEntities).ToList();
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
