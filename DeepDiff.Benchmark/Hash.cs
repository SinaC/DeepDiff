using BenchmarkDotNet.Attributes;
using DeepDiff.Benchmark.Entities;
using DeepDiff.Configuration;
using DeepDiff.Internal.Comparers;
using DeepDiff.Internal.Configuration;

namespace DeepDiff.Benchmark;

public class Hash
{
    private Random Random { get; }
    private IReadOnlyCollection<NavigationEntityLevel1> Entities { get; set; } = null!;
    
    private IComparerByProperty NaiveHash1Property { get; }
    private IComparerByProperty NaiveHash4Properties { get; }
    private IComparerByProperty PrecompiledHash1Property { get; }
    private IComparerByProperty PrecompiledHash4Properties { get; }

    public Hash()
    {
        Random = new Random();

        var entityConfiguration1Property = new EntityConfiguration<NavigationEntityLevel1>(new EntityConfiguration(typeof(NavigationEntityLevel1)));
        entityConfiguration1Property.HasKey(x => x.Timestamp);

        NaiveHash1Property = new NaiveEqualityComparerByProperty<NavigationEntityLevel1>(entityConfiguration1Property.Configuration.KeyConfiguration.KeyProperties);
        PrecompiledHash1Property = new PrecompiledEqualityComparerByProperty<NavigationEntityLevel1>(entityConfiguration1Property.Configuration.KeyConfiguration.KeyProperties.Select(x => x.PropertyInfo).ToArray());

        var entityConfiguration4Properties = new EntityConfiguration<NavigationEntityLevel1>(new EntityConfiguration(typeof(NavigationEntityLevel1)));
        entityConfiguration4Properties.HasKey(x => new { x.Id, x.Timestamp, x.Power, x.Comment });

        NaiveHash4Properties = new NaiveEqualityComparerByProperty<NavigationEntityLevel1>(entityConfiguration4Properties.Configuration.KeyConfiguration.KeyProperties);
        PrecompiledHash4Properties = new PrecompiledEqualityComparerByProperty<NavigationEntityLevel1>(entityConfiguration4Properties.Configuration.KeyConfiguration.KeyProperties.Select(x => x.PropertyInfo).ToArray());

    }

    [Params(10000, 100000, 1000000)]
    public int N { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        GenerateRandom();
    }

    [Benchmark]
    public void NaiveHash1PropertyGetHashCode()
    {
        foreach (var entity in Entities)
        {
            var hashCode = NaiveHash1Property.GetHashCode(entity);
        }
    }

    [Benchmark]
    public void PrecompiledHash1PropertyGetHashCode()
    {
        foreach (var entity in Entities)
        {
            var hashCode = PrecompiledHash1Property.GetHashCode(entity);
        }
    }

    [Benchmark]
    public void NaiveHash4PropertiesGetHashCode()
    {
        foreach (var entity in Entities)
        {
            var hashCode = NaiveHash4Properties.GetHashCode(entity);
        }
    }

    [Benchmark]
    public void PrecompiledHash4Properties_GetHashCode()
    {
        foreach (var entity in Entities)
        {
            var hashCode = PrecompiledHash4Properties.GetHashCode(entity);
        }
    }

    private void GenerateRandom()
    {
        Entities = Enumerable.Range(0, N)
            .Select(x => new NavigationEntityLevel1
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Today.AddMicroseconds(x),
                Power = x,
                Comment = "Comment_" + (x % 1000),
            }).ToArray();
    }
}
