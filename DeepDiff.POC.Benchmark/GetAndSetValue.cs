using BenchmarkDotNet.Attributes;
using DeepDiff.POC.Benchmark.Entities;
using DeepDiff.POC.Benchmark.Helpers;
using DeepDiff.POC.Comparers;
using System.Reflection;

namespace DeepDiff.POC.Benchmark;

public class GetAndSetValue
{
    private IReadOnlyCollection<NavigationEntityLevel1> Entities { get; set; } = null!;

    private PropertyInfo TimestampProperty { get; }
    private PropertyInfoExt TimestampPropertyExt { get; }

    public GetAndSetValue()
    {
        var factory = new ComparerFactory<NavigationEntityLevel1>();
        TimestampProperty = factory.GetPropertyInfo(e => e.Timestamp);
        TimestampPropertyExt = new PropertyInfoExt(typeof(NavigationEntityLevel1), TimestampProperty);
    }

    [Params(10000, 100000, 1000000)]
    public int N { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        Generate();
    }

    [Benchmark]
    public void PropertyInfo_GetValue()
    {
        foreach (var entity in Entities)
        {
            var value = TimestampProperty.GetValue(entity);
        }
    }

    [Benchmark]
    public void PropertyInfoExt_GetValue()
    {
        foreach (var entity in Entities)
        {
            var value = TimestampPropertyExt.GetValue(entity);
        }
    }

    [Benchmark]
    public void PropertyInfo_SetValue()
    {
        int n = 0;
        foreach (var entity in Entities)
        {
            var timestamp = DateTime.Today.AddMicroseconds(n);
            TimestampProperty.SetValue(entity, timestamp);
            n++;
        }
    }

    [Benchmark]
    public void PropertyInfoExt_SetValue()
    {
        int n = 0;
        foreach (var entity in Entities)
        {
            var timestamp = DateTime.Today.AddMicroseconds(n);
            TimestampPropertyExt.SetValue(entity, timestamp);
            n++;
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
