using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class CompareSingleConfiguration : ICompareSingleConfiguration
    {
        public DiffEngineConfiguration Configuration { get; }

        public CompareSingleConfiguration()
        {
            Configuration = new DiffEngineConfiguration();
            Configuration.SetForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(false);
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

        public ICompareSingleConfiguration UsePrecompiledEqualityComparer(bool use = true)
        {
            Configuration.SetUsePrecompiledEqualityComparer(use);
            return this;
        }
    }
}
