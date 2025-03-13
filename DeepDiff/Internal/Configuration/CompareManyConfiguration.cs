using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class CompareManyConfiguration : ICompareManyConfiguration
    {
        public DiffEngineConfiguration Configuration { get; }

        public CompareManyConfiguration()
        {
            Configuration = new DiffEngineConfiguration();
            Configuration.SetForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(false);
            Configuration.SetCompareOnly(true);
        }

        public ICompareManyConfiguration UseHashtable(bool use = true)
        {
            Configuration.SetUseHashtable(use);
            return this;
        }

        public ICompareManyConfiguration HashtableThreshold(int threshold = 15)
        {
            Configuration.SetHashtableThreshold(threshold);
            return this;
        }

        public ICompareManyConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled)
        {
            Configuration.SetEqualityComparer(equalityComparer);
            return this;
        }

        public ICompareManyConfiguration UseParallelism(bool use = false)
        {
            Configuration.SetUseParallelism(use);
            return this;
        }
    }
}
