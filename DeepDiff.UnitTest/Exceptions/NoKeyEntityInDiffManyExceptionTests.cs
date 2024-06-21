using DeepDiff.Configuration;
using DeepDiff.Exceptions;
using DeepDiff.UnitTest.Entities.Simple;
using System;
using System.Linq;
using Xunit;

namespace DeepDiff.UnitTest.Exceptions
{
    public class NoKeyEntityInDiffManyExceptionTests
    {
        [Fact]
        public void DiffMany()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel1>()
                .NoKey();
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var existingEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel1
            {
                Index = x,

                Timestamp = DateTime.Now.AddMinutes(15*x),

                Power = x,
                Price = 2*x
            }).ToArray();

            var newEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel1
            {
                Index = x,

                Timestamp = DateTime.Now.AddMinutes(15 * x),

                Power = x + 1,
                Price = 2 * x + 1
            }).ToArray();

            Assert.Throws<NoKeyEntityInDiffManyException>(() => deepDiff.MergeMany(existingEntities, newEntities));
        }

        [Fact]
        public void MergeSingle()
        {
            var diffConfiguration = new DeepDiffConfiguration();
            diffConfiguration.Entity<EntityLevel1>()
                .HasValues(x => new { x.Power, x.Price })
                .NoKey();
            var deepDiff = diffConfiguration.CreateDeepDiff();

            var existingEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel1
            {
                Index = x,

                Timestamp = DateTime.Now.AddMinutes(15 * x),

                Power = x,
                Price = 2 * x
            }).ToArray();

            var newEntities = Enumerable.Range(0, 10).Select(x => new EntityLevel1
            {
                Index = x,

                Timestamp = DateTime.Now.AddMinutes(15 * x),

                Power = x + 1,
                Price = 2 * x + 1
            }).ToArray();

            var diff =  deepDiff.MergeSingle(existingEntities[0], newEntities[0]);
            Assert.NotNull(diff.Entity);
        }
    }
}
