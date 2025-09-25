using BenchmarkDotNet.Attributes;
using DeepDiff.Benchmark.Entities;
using DeepDiff.Internal.Comparers;
using DeepDiff.Internal.Configuration;

namespace DeepDiff.Benchmark;

public class Hash
{
    private IReadOnlyCollection<NavigationEntityLevel1> Entities { get; set; } = null!;
    
    private IComparerByProperty NaiveComparer1Property { get; }
    private IComparerByProperty NaiveComparer4Properties { get; }
    private IComparerByProperty PrecompiledComparer1Property { get; }
    private IComparerByProperty PrecompiledComparer4Properties { get; }

    public Hash()
    {
        var entityConfiguration1Property = new EntityConfiguration<NavigationEntityLevel1>(new EntityConfiguration(typeof(NavigationEntityLevel1)));
        entityConfiguration1Property.HasKey(x => x.Timestamp);

        NaiveComparer1Property = new NaiveEqualityComparerByProperty<NavigationEntityLevel1>(entityConfiguration1Property.Configuration.KeyConfiguration.KeyProperties);
        PrecompiledComparer1Property = new PrecompiledEqualityComparerByProperty<NavigationEntityLevel1>(entityConfiguration1Property.Configuration.KeyConfiguration.KeyProperties.Select(x => x.PropertyInfo).ToArray());

        var entityConfiguration4Properties = new EntityConfiguration<NavigationEntityLevel1>(new EntityConfiguration(typeof(NavigationEntityLevel1)));
        entityConfiguration4Properties.HasKey(x => new { x.Id, x.Timestamp, x.Power, x.Comment });

        NaiveComparer4Properties = new NaiveEqualityComparerByProperty<NavigationEntityLevel1>(entityConfiguration4Properties.Configuration.KeyConfiguration.KeyProperties);
        PrecompiledComparer4Properties = new PrecompiledEqualityComparerByProperty<NavigationEntityLevel1>(entityConfiguration4Properties.Configuration.KeyConfiguration.KeyProperties.Select(x => x.PropertyInfo).ToArray());

    }

    [Params(10000, 100000, 1000000)]
    public int N { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        Generate();
    }

    [Benchmark]
    public void Naive1PropertyGetHashCode()
    {
        Test_GetHashCode(NaiveComparer1Property);
    }

    [Benchmark]
    public void Precompiled1Property_GetHashCode()
    {
        Test_GetHashCode(PrecompiledComparer1Property);
    }

    [Benchmark]
    public void Naive4Properties_GetHashCode()
    {
        Test_GetHashCode(NaiveComparer4Properties);
    }

    [Benchmark]
    public void Precompiled4Properties_GetHashCode()
    {
        Test_GetHashCode(PrecompiledComparer4Properties);
    }

    private void Test_GetHashCode(IComparerByProperty comparer)
    {
        foreach (var entity in Entities)
        {
            var hashCode = comparer.GetHashCode(entity);
        }
    }

    private void Generate()
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
