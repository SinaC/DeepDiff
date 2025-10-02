using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class MergeSingleConfiguration : IMergeSingleConfiguration
    {
        public DiffEngineConfiguration Configuration { get; }

        public MergeSingleConfiguration()
        {
            Configuration = new DiffEngineConfiguration();
        }

        public IMergeSingleConfiguration UseHashtable(bool use = true)
        {
            Configuration.SetUseHashtable(use);
            return this;
        }

        public IMergeSingleConfiguration HashtableThreshold(int threshold = 15)
        {
            Configuration.SetHashtableThreshold(threshold);
            return this;
        }

        public IMergeSingleConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled)
        {
            Configuration.SetEqualityComparer(equalityComparer);
            return this;
        }

        public IMergeSingleConfiguration ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(bool force = false)
        {
            Configuration.SetForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(force);
            return this;
        }

        public IMergeSingleConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true)
        {
            Configuration.SetCheckDuplicateKeys(checkDuplicateKeys);
            return this;
        }
    }
}
