using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class DiffManyConfiguration : IDiffManyConfiguration
    {
        public DiffEngineConfiguration Configuration { get; }

        public DiffManyConfiguration()
        {
            Configuration = new DiffEngineConfiguration();
            Configuration.SetForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(false);
            Configuration.SetGenerateOperations(DiffOperations.All);
            Configuration.SetGenerateOperationsOnly(true);
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

        public IDiffManyConfiguration UsePrecompiledEqualityComparer(bool use = true)
        {
            Configuration.SetUsePrecompiledEqualityComparer(use);
            return this;
        }

        public IDiffManyConfiguration GenerateOperations(DiffOperations operationsToGenerate = DiffOperations.All)
        {
            if (operationsToGenerate != DiffOperations.None)
                Configuration.SetGenerateOperations(operationsToGenerate);
            return this;
        }
    }
}
