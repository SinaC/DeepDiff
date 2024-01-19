using DeepDiff.Comparers;
using DeepDiff.Configuration;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace DeepDiff.UnitTest.Performance
{
    public class HashPerformanceTests
    {
        private ITestOutputHelper Output { get; }

        public HashPerformanceTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void NaiveEqualityComparerByProperty_1Field()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var entities = Enumerable.Range(0, 1000000).Select(x => new EntityLevel1
            {
                Timestamp = DateTime.Now.AddSeconds(x),
            }).ToList();
            sw.Stop();
            Output.WriteLine("Generation: {0} ms", sw.ElapsedMilliseconds);

            var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
            diffEntityConfiguration.HasKey(x => x.Timestamp);
            var comparer = new NaiveEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.KeyConfiguration.KeyProperties);

            sw.Restart();
            foreach(var entity in entities)
                comparer.GetHashCode(entity);
            sw.Stop();
            Output.WriteLine("Compare: {0} ms", sw.ElapsedMilliseconds);
        }

        [Fact]
        public void PrecompiledEqualityComparerByProperty_1Field()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var entities = Enumerable.Range(0, 1000000).Select(x => new EntityLevel1
            {
                Timestamp = DateTime.Now.AddSeconds(x),
            }).ToList();
            sw.Stop();
            Output.WriteLine("Generation: {0} ms", sw.ElapsedMilliseconds);

            var diffEntityConfiguration = new DiffEntityConfiguration<EntityLevel1>(new DiffEntityConfiguration(typeof(EntityLevel1)));
            diffEntityConfiguration.HasKey(x => x.Timestamp);
            var comparer = new PrecompiledEqualityComparerByProperty<EntityLevel1>(diffEntityConfiguration.Configuration.KeyConfiguration.KeyProperties);

            sw.Restart();
            foreach (var entity in entities)
                comparer.GetHashCode(entity);
            sw.Stop();
            Output.WriteLine("Compare: {0} ms", sw.ElapsedMilliseconds);
        }

        [Fact]
        public void NaiveEqualityComparerByProperty_4Fields()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var entities = Enumerable.Range(0, 1000000).Select(x => new EntityLevel1
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
            foreach (var entity in entities)
                comparer.GetHashCode(entity);
            sw.Stop();
            Output.WriteLine("Compare: {0} ms", sw.ElapsedMilliseconds);
        }

        [Fact]
        public void PrecompiledEqualityComparerByProperty_4Fields()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var entities = Enumerable.Range(0, 1000000).Select(x => new EntityLevel1
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
            foreach (var entity in entities)
                comparer.GetHashCode(entity);
            sw.Stop();
            Output.WriteLine("Compare: {0} ms", sw.ElapsedMilliseconds);
        }
    }
}
