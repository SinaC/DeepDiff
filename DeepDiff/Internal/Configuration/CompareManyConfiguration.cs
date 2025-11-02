using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class CompareManyConfiguration : ICompareManyConfiguration
    {
        public DiffEngineConfiguration Configuration { get; }

        public CompareManyConfiguration()
        {
            Configuration = new DiffEngineConfiguration();
            Configuration.SetForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(false);
            Configuration.SetCompareOnly(true);
        }

        public ICompareManyConfiguration HashtableThreshold(int threshold = 15)
        {
            Configuration.SetHashtableThreshold(threshold);
            return this;
        }

        public ICompareManyConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true)
        {
            Configuration.SetCheckDuplicateKeys(checkDuplicateKeys);
            return this;
        }
    }
}
