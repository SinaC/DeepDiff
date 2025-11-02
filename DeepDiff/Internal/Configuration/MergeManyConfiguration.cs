using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class MergeManyConfiguration : IMergeManyConfiguration
    {
        public DiffEngineConfiguration Configuration { get; }

        public MergeManyConfiguration()
        {
            Configuration = new DiffEngineConfiguration();
        }

        public IMergeManyConfiguration HashtableThreshold(int threshold = 15)
        {
            Configuration.SetHashtableThreshold(threshold);
            return this;
        }

        public IMergeManyConfiguration ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(bool force = false)
        {
            Configuration.SetForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(force);
            return this;
        }

        public IMergeManyConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true)
        {
            Configuration.SetCheckDuplicateKeys(checkDuplicateKeys);
            return this;
        }
    }
}
