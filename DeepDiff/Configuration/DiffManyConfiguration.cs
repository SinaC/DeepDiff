namespace DeepDiff.Configuration
{
    internal sealed class DiffManyConfiguration : IDiffManyConfiguration
    {
        public DiffEngineConfiguration Configuration { get; }

        public DiffManyConfiguration()
        {
            Configuration = new DiffEngineConfiguration();
        }

        public IDiffManyConfiguration UseHashtable(bool use = true)
        {
            Configuration.SetUseHashtable(use);
            return this;
        }

        public IDiffManyConfiguration HashtableThreshold(int threshold = 15)
        {
            Configuration.SetHashtableThreshold(threshold);
            return this;
        }

        public IDiffManyConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(bool force = false)
        {
            Configuration.SetForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(force);
            return this;
        }

        public IDiffManyConfiguration GenerateOperations(bool generate = true)
        {
            Configuration.SetGenerateOperations(generate);
            return this;
        }

        public IDiffManyConfiguration OnlyGenerateOperations(bool onlyGenerate = false)
        {
            Configuration.SetGenerateOperationsOnly(onlyGenerate);
            return this;
        }

        public IDiffManyConfiguration UsePrecompiledEqualityComparer(bool use = true)
        {
            Configuration.SetUsePrecompiledEqualityComparer(use);
            return this;
        }
    }
}
