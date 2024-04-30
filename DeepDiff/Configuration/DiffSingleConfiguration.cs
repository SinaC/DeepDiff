namespace DeepDiff.Configuration
{
    internal sealed class DiffSingleConfiguration : IDiffSingleConfiguration
    {
        public DiffEngineConfiguration Configuration { get; }

        public DiffSingleConfiguration()
        {
            Configuration = new DiffEngineConfiguration();
        }

        public IDiffSingleConfiguration UseHashtable(bool use = true)
        {
            Configuration.SetUseHashtable(use);
            return this;
        }

        public IDiffSingleConfiguration HashtableThreshold(int threshold = 15)
        {
            Configuration.SetHashtableThreshold(threshold);
            return this;
        }

        public IDiffSingleConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(bool force = false)
        {
            Configuration.SetForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(force);
            return this;
        }

        public IDiffSingleConfiguration GenerateOperations(bool generate = true)
        {
            Configuration.SetGenerateOperations(generate);
            return this;
        }

        public IDiffSingleConfiguration OnlyGenerateOperations(bool onlyGenerate = false)
        {
            Configuration.SetGenerateOperationsOnly(onlyGenerate);
            return this;
        }

        public IDiffSingleConfiguration UsePrecompiledEqualityComparer(bool use = true)
        {
            Configuration.SetUsePrecompiledEqualityComparer(use);
            return this;
        }
    }
}
