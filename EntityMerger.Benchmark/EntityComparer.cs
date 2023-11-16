using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using EntityMerger.EntityMerger;

namespace EntityMerger.Benchmark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    public class EntityComparer
    {
        private NoNavigationEntity[] ExistingEntities { get; set; } = null!;
        private NoNavigationEntity[] CalculatedEntities { get; set; } = null!;

        private IMerger Merger { get; }
        private IHashMerger HashMerger { get; }

        public EntityComparer()
        {
            var mergeConfiguration = new MergeConfiguration();
            mergeConfiguration
                .Entity<NoNavigationEntity>()
                .HasKey(x => new { x.Date, x.ContractReference })
                .HasCalculatedValue(x => new { x.Penalty, x.Volume, x.Price })
                .MarkAsInserted(x => x.PersistChange, PersistChange.Insert)
                .MarkAsUpdated(x => x.PersistChange, PersistChange.Update)
                .MarkAsDeleted(x => x.PersistChange, PersistChange.Delete);
            Merger = mergeConfiguration.CreateMerger();
            HashMerger = mergeConfiguration.CreateHashMerger();
        }

        [Params(10, 1000, 1000000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            ExistingEntities = Enumerable.Range(0, N)
               .Select(x => new NoNavigationEntity
               {
                   Id = Guid.NewGuid(),
                   Date = DateTime.Today,
                   ContractReference = "REF",
                   Price = x,
                   Penalty = 2 * x,
                   Volume = x
               }).ToArray();
            CalculatedEntities = Enumerable.Range(0, N)
                .Select(x => new NoNavigationEntity
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Today,
                    ContractReference = "REF",
                    Price = x,
                    Penalty = 2 * x,
                    Volume = x
                }).ToArray();
        }

        [Benchmark]
        public void EqualsHash()
        {
            for (int i = 0; i < N; i++)
                HashMerger.Equals(ExistingEntities[i], CalculatedEntities[i]);
        }

        [Benchmark]
        public void EqualsNoHash()
        {
            for (int i = 0; i < N; i++)
                Merger.Equals(ExistingEntities[i], CalculatedEntities[i]);
        }
    }
}
