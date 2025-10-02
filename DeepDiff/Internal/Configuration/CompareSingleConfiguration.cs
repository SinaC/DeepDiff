using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class CompareSingleConfiguration : ICompareSingleConfiguration
    {
        public DiffEngineConfiguration Configuration { get; }

        public CompareSingleConfiguration()
        {
            Configuration = new DiffEngineConfiguration();
            Configuration.SetForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(false);
            Configuration.SetCompareOnly(true);
        }

        public ICompareSingleConfiguration UseHashtable(bool use = true)
        {
            Configuration.SetUseHashtable(use);
            return this;
        }

        public ICompareSingleConfiguration HashtableThreshold(int threshold = 15)
        {
            Configuration.SetHashtableThreshold(threshold);
            return this;
        }

        public ICompareSingleConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled)
        {
            Configuration.SetEqualityComparer(equalityComparer);
            return this;
        }

        public ICompareSingleConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true)
        {
            Configuration.SetCheckDuplicateKeys(checkDuplicateKeys);
            return this;
        }
    }
}
