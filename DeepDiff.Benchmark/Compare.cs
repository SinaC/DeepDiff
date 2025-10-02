using BenchmarkDotNet.Attributes;
using DeepDiff.Benchmark.Entities;
using DeepDiff.Internal.Comparers;
using DeepDiff.Internal.Configuration;

namespace DeepDiff.Benchmark;

public class Compare
{
    private Random Random { get; }
    private IReadOnlyCollection<NavigationEntityLevel1> ExistingEntities { get; set; } = null!;
    private IReadOnlyCollection<NavigationEntityLevel1> NewEntities { get; set; } = null!;

    private IComparerByProperty NaiveComparer1Property { get; }
    private IComparerByProperty NaiveComparer4Properties { get; }
    private IComparerByProperty PrecompiledComparer1Property { get; }
    private IComparerByProperty PrecompiledComparer4Properties { get; }

    public Compare()
    {
        Random = new Random();

        var entityConfiguration1Property = new EntityConfiguration<NavigationEntityLevel1>(new EntityConfiguration(typeof(NavigationEntityLevel1)));
        entityConfiguration1Property.HasKey(x => x.Timestamp);

        NaiveComparer1Property = new NaiveEqualityComparerByProperty<NavigationEntityLevel1>(entityConfiguration1Property.Configuration.KeyConfiguration.KeyProperties);
        PrecompiledComparer1Property = new PrecompiledEqualityComparerByProperty<NavigationEntityLevel1>(entityConfiguration1Property.Configuration.KeyConfiguration.KeyProperties.Select(x => x.PropertyInfo).ToArray());

        var entityConfiguration4Properties = new EntityConfiguration<NavigationEntityLevel1>(new EntityConfiguration(typeof(NavigationEntityLevel1)));
        entityConfiguration4Properties.HasKey(x => new { x.Id, x.Timestamp, x.Power, x.Comment });

        NaiveComparer4Properties = new NaiveEqualityComparerByProperty<NavigationEntityLevel1>(entityConfiguration4Properties.Configuration.KeyConfiguration.KeyProperties);
        PrecompiledComparer4Properties = new PrecompiledEqualityComparerByProperty<NavigationEntityLevel1>(entityConfiguration4Properties.Configuration.KeyConfiguration.KeyProperties.Select(x => x.PropertyInfo).ToArray());

    }

    [Params(100, 1000, 10000)]
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
    public void Naive1Property_Equals()
    {
        Test_Equals(NaiveComparer1Property);
    }

    [Benchmark]
    public void Precompiled1Property_Equals()
    {
        Test_Equals(PrecompiledComparer1Property);
    }

    [Benchmark]
    public void Naive4Properties_Equals()
    {
        Test_Equals(NaiveComparer4Properties);
    }

    [Benchmark]
    public void Precompiled4Properties_Equals()
    {
        Test_Equals(PrecompiledComparer4Properties);
    }

    private void Test_Equals(IComparerByProperty comparer)
    {
        foreach (var existingEntity in ExistingEntities)
        {
            foreach (var newEntity in NewEntities)
            {
                var compare = comparer.Equals(existingEntity, newEntity);
            }
        }
    }

    private void GenerateIdentical()
    {
        ExistingEntities = Enumerable.Range(0, N)
            .Select(x => new NavigationEntityLevel1
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMicroseconds(x),
                Power = x,
                Comment = "Comment_" + (x % 1000),
            }).ToArray();
        NewEntities = Enumerable.Range(0, N)
            .Select(x => new NavigationEntityLevel1
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMicroseconds(x),
                Power = x,
                Comment = "Comment_" + (x % 1000),
            }).ToArray();
    }

    private void GenerateNoExisting()
    {
        ExistingEntities = Array.Empty<NavigationEntityLevel1>();
        NewEntities = Enumerable.Range(0, N)
            .Select(x => new NavigationEntityLevel1
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMicroseconds(x),
                Power = x,
                Comment = "Comment_" + (x % 1000),
            }).ToArray();
    }

    private void GenerateNoNew()
    {
        ExistingEntities = Enumerable.Range(0, N)
            .Select(x => new NavigationEntityLevel1
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMicroseconds(x),
                Power = x,
                Comment = "Comment_" + (x % 1000),
            }).ToArray();
        NewEntities = Array.Empty<NavigationEntityLevel1>();
    }

    private void GenerateRandom()
    {
        const int SampleSize = 10;
        var guids = Enumerable.Range(0, SampleSize).Select(x => Guid.NewGuid()).ToArray();
        var timestamps = Enumerable.Range(0, SampleSize).Select(x => DateTime.Today.AddMicroseconds(x)).ToArray();
        var powers = Enumerable.Range(0, SampleSize).ToArray();
        ExistingEntities = Enumerable.Range(0, N)
            .Select(x => new NavigationEntityLevel1
            {
                Id = guids[Random.Next(SampleSize)],
                Timestamp = timestamps[Random.Next(SampleSize)],
                Power = powers[Random.Next(SampleSize)],
                Comment = "Comment_" + (x % 1000),
            }).ToArray();
        NewEntities = Enumerable.Range(0, N)
            .Select(x => new NavigationEntityLevel1
            {
                Id = guids[Random.Next(SampleSize)],
                Timestamp = timestamps[Random.Next(SampleSize)],
                Power = powers[Random.Next(SampleSize)],
                Comment = "Comment_" + (x % 1000),
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
