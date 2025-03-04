using DeepDiff.Internal.Comparers;
using DeepDiff.Internal.Configuration;
using DeepDiff.PerformanceTest.Entities.Simple;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace DeepDiff.PerformanceTest.Performance;

public class HashPerformanceTests
{
    private const int EntityCount = 10000000;

    private static IReadOnlyCollection<EntityLevel1> Entities { get; } = Enumerable.Range(0, EntityCount).Select(x => new EntityLevel1
    {
        Timestamp = DateTime.Now.AddSeconds(x),
        Price = x,
        Power = 2 * x,
        Comment = "Comment"
    }).ToList();

    private ITestOutputHelper Output { get; }

    public HashPerformanceTests(ITestOutputHelper output)
    {
        Output = output;
    }

    [Fact]
    public void NaiveEqualityComparerByProperty_1Field()
    {
        var entityConfiguration = new EntityConfiguration<EntityLevel1>(new EntityConfiguration(typeof(EntityLevel1)));
        entityConfiguration.HasKey(x => x.Timestamp);
        var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(entityConfiguration.Configuration.KeyConfiguration.KeyProperties);

        TestGetHashCode(comparer);
    }

    [Fact]
    public void PrecompiledEqualityComparerByProperty_1Field()
    {
        var entityConfiguration = new EntityConfiguration<EntityLevel1>(new EntityConfiguration(typeof(EntityLevel1)));
        entityConfiguration.HasKey(x => x.Timestamp);
        var comparer = new PrecompiledEqualityComparerByProperty<EntityLevel1>(entityConfiguration.Configuration.KeyConfiguration.KeyProperties);

        TestGetHashCode(comparer);
    }

    [Fact]
    public void NaiveEqualityComparerByProperty_4Fields()
    {
        var entityConfiguration = new EntityConfiguration<EntityLevel1>(new EntityConfiguration(typeof(EntityLevel1)));
        entityConfiguration.HasKey(x => new { x.Timestamp, x.Price, x.Power, x.Comment });
        var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(entityConfiguration.Configuration.KeyConfiguration.KeyProperties);

        TestGetHashCode(comparer);
    }

    [Fact]
    public void PrecompiledEqualityComparerByProperty_4Fields()
    {
        var entityConfiguration = new EntityConfiguration<EntityLevel1>(new EntityConfiguration(typeof(EntityLevel1)));
        entityConfiguration.HasKey(x => new { x.Timestamp, x.Price, x.Power, x.Comment });
        var comparer = new PrecompiledEqualityComparerByProperty<EntityLevel1>(entityConfiguration.Configuration.KeyConfiguration.KeyProperties);

        TestGetHashCode(comparer);
    }

    private void TestGetHashCode(IComparerByProperty comparer)
    {
        var sw = Stopwatch.StartNew();
        foreach (var entity in Entities)
            comparer.GetHashCode(entity);
        sw.Stop();
        Output.WriteLine("Compare: {0} ms", sw.ElapsedMilliseconds);
    }
}
