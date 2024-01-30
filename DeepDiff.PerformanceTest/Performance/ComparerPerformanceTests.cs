using DeepDiff.Comparers;
using DeepDiff.Configuration;
using DeepDiff.PerformanceTest.Entities.Simple;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace DeepDiff.PerformanceTest;

public class ComparerPerformanceTests
{
    private ITestOutputHelper Output { get; }

    public ComparerPerformanceTests(ITestOutputHelper output)
    {
        Output = output;
    }

    [Fact]
    public void NaiveEqualityComparerByProperty_1Field()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        var existingEntities = Enumerable.Range(0, 10000).Select(x => new EntityLevel1
        {
            Timestamp = DateTime.Now.AddSeconds(x),
        }).ToList();
        var newEntities = Enumerable.Range(0, 10000).Select(x => new EntityLevel1
        {
            Timestamp = DateTime.Now.AddSeconds(x),
        }).ToList();
        sw.Stop();
        Output.WriteLine("Generation: {0} ms", sw.ElapsedMilliseconds);

        var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
        diffEntityConfiguration.HasKey(x => x.Timestamp);
        var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.KeyConfiguration.KeyProperties);

        sw.Restart();
        foreach (var existingEntity in existingEntities)
        {
            foreach (var newEntity in newEntities)
            {
                var compare = comparer.Equals(existingEntity, newEntity);
            }
        }
        sw.Stop();
        Output.WriteLine("Compare: {0} ms", sw.ElapsedMilliseconds);
    }

    [Fact]
    public void PrecompiledEqualityComparerByProperty_1Field()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        var existingEntities = Enumerable.Range(0, 10000).Select(x => new EntityLevel1
        {
            Timestamp = DateTime.Now.AddSeconds(x),
        }).ToList();
        var newEntities = Enumerable.Range(0, 10000).Select(x => new EntityLevel1
        {
            Timestamp = DateTime.Now.AddSeconds(x),
        }).ToList();
        sw.Stop();
        Output.WriteLine("Generation: {0} ms", sw.ElapsedMilliseconds);

        var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
        diffEntityConfiguration.HasKey(x => x.Timestamp);
        var comparer = new PrecompiledEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.KeyConfiguration.KeyProperties);

        sw.Restart();
        foreach (var existingEntity in existingEntities)
        {
            foreach (var newEntity in newEntities)
            {
                var compare = comparer.Equals(existingEntity, newEntity);
            }
        }
        sw.Stop();
        Output.WriteLine("Compare: {0} ms", sw.ElapsedMilliseconds);
    }

    [Fact]
    public void NaiveEqualityComparerByProperty_4Fields()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        var existingEntities = Enumerable.Range(0, 10000).Select(x => new EntityLevel1
        {
            Timestamp = DateTime.Now.AddSeconds(x),
            Price = x,
            Power = 2 * x,
            Comment = "Comment"
        }).ToList();
        var newEntities = Enumerable.Range(0, 10000).Select(x => new EntityLevel1
        {
            Timestamp = DateTime.Now.AddSeconds(x),
            Price = x,
            Power = 2 * x,
            Comment = "Comment"
        }).ToList();
        sw.Stop();
        Output.WriteLine("Generation: {0} ms", sw.ElapsedMilliseconds);

        var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
        diffEntityConfiguration.HasKey(x => new { x.Timestamp, x.Price, x.Power, x.Comment });
        var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.KeyConfiguration.KeyProperties);

        sw.Restart();
        foreach (var existingEntity in existingEntities)
        {
            foreach (var newEntity in newEntities)
            {
                var compare = comparer.Equals(existingEntity, newEntity);
            }
        }
        sw.Stop();
        Output.WriteLine("Compare: {0} ms", sw.ElapsedMilliseconds);
    }

    [Fact]
    public void PrecompiledEqualityComparerByProperty_4Fields()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        var existingEntities = Enumerable.Range(0, 10000).Select(x => new EntityLevel1
        {
            Timestamp = DateTime.Now.AddSeconds(x),
            Price = x,
            Power = 2 * x,
            Comment = "Comment"
        }).ToList();
        var newEntities = Enumerable.Range(0, 10000).Select(x => new EntityLevel1
        {
            Timestamp = DateTime.Now.AddSeconds(x),
            Price = x,
            Power = 2 * x,
            Comment = "Comment"
        }).ToList();
        sw.Stop();
        Output.WriteLine("Generation: {0} ms", sw.ElapsedMilliseconds);

        var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
        diffEntityConfiguration.HasKey(x => new { x.Timestamp, x.Price, x.Power, x.Comment });
        var comparer = new PrecompiledEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.KeyConfiguration.KeyProperties);

        sw.Restart();
        foreach (var existingEntity in existingEntities)
        {
            foreach (var newEntity in newEntities)
            {
                var compare = comparer.Equals(existingEntity, newEntity);
            }
        }
        sw.Stop();
        Output.WriteLine("Compare: {0} ms", sw.ElapsedMilliseconds);
    }

    [Fact]
    public void NaiveEqualityComparerByProperty_4Fields_CustomComparer()
    {
        var typeSpecificComparers = new Dictionary<Type, object>
            {
                { typeof(decimal?), new NullableDecimalComparer(6) },
                { typeof(decimal), new DecimalComparer(6) }
            };

        Stopwatch sw = new Stopwatch();
        sw.Start();
        var existingEntities = Enumerable.Range(0, 10000).Select(x => new EntityLevel1
        {
            Timestamp = DateTime.Now.AddSeconds(x),
            Price = x,
            Power = 2 * x,
            Comment = "Comment"
        }).ToList();
        var newEntities = Enumerable.Range(0, 10000).Select(x => new EntityLevel1
        {
            Timestamp = DateTime.Now.AddSeconds(x),
            Price = x,
            Power = 2 * x,
            Comment = "Comment"
        }).ToList();
        sw.Stop();
        Output.WriteLine("Generation: {0} ms", sw.ElapsedMilliseconds);

        var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
        diffEntityConfiguration.HasKey(x => new { x.Timestamp, x.Price, x.Power, x.Comment });
        var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.KeyConfiguration.KeyProperties, typeSpecificComparers, null);

        sw.Restart();
        foreach (var existingEntity in existingEntities)
        {
            foreach (var newEntity in newEntities)
            {
                var compare = comparer.Equals(existingEntity, newEntity);
            }
        }
        sw.Stop();
        Output.WriteLine("Compare: {0} ms", sw.ElapsedMilliseconds);
    }

    [Fact]
    public void PrecompiledEqualityComparerByProperty_4Fields_CustomComparer()
    {
        var typeSpecificComparers = new Dictionary<Type, object>
            {
                { typeof(decimal?), new NullableDecimalComparer(6) },
                { typeof(decimal), new DecimalComparer(6) }
            };

        Stopwatch sw = new Stopwatch();
        sw.Start();
        var existingEntities = Enumerable.Range(0, 10000).Select(x => new EntityLevel1
        {
            Timestamp = DateTime.Now.AddSeconds(x),
            Price = x,
            Power = 2 * x,
            Comment = "Comment"
        }).ToList();
        var newEntities = Enumerable.Range(0, 10000).Select(x => new EntityLevel1
        {
            Timestamp = DateTime.Now.AddSeconds(x),
            Price = x,
            Power = 2 * x,
            Comment = "Comment"
        }).ToList();
        sw.Stop();
        Output.WriteLine("Generation: {0} ms", sw.ElapsedMilliseconds);

        var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
        diffEntityConfiguration.HasKey(x => new { x.Timestamp, x.Price, x.Power, x.Comment });
        var comparer = new PrecompiledEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.KeyConfiguration.KeyProperties, typeSpecificComparers, null);

        sw.Restart();
        foreach (var existingEntity in existingEntities)
        {
            foreach (var newEntity in newEntities)
            {
                var compare = comparer.Equals(existingEntity, newEntity);
            }
        }
        sw.Stop();
        Output.WriteLine("Compare: {0} ms", sw.ElapsedMilliseconds);
    }
}
