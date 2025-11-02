using BenchmarkDotNet.Attributes;
using DeepDiff.POC.Benchmark.Entities;
using DeepDiff.POC.Benchmark.Helpers;
using DeepDiff.POC.Comparers;

namespace DeepDiff.POC.Benchmark;

public class Hash
{
    private IReadOnlyCollection<NavigationEntityLevel1> Entities { get; set; } = null!;

    private IComparerByProperty NaiveComparer1Property { get; }
    private IComparerByProperty NaiveComparer4Properties { get; }
    private IComparerByProperty PrecompiledComparer1Property { get; }
    private IComparerByProperty PrecompiledComparer4Properties { get; }

    public Hash()
    {
        var factory = new ComparerFactory<NavigationEntityLevel1>();
        NaiveComparer1Property = factory.CreateNaiveComparer(x => x.Timestamp);
        PrecompiledComparer1Property = factory.CreatePrecompiledComparer(x => x.Timestamp);
        NaiveComparer4Properties = factory.CreateNaiveComparer(x => new { x.Id, x.Timestamp, x.Power, x.Comment });
        PrecompiledComparer4Properties = factory.CreatePrecompiledComparer(x => new { x.Id, x.Timestamp, x.Power, x.Comment });
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
