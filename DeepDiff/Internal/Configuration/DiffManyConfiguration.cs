using DeepDiff.Configuration;
using Config = DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class DiffManyConfiguration : IDiffManyConfiguration
    {
        public DiffEngineConfiguration Configuration { get; }

        public DiffManyConfiguration()
        {
            Configuration = new DiffEngineConfiguration();
            Configuration.SetForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(false);
            Configuration.SetGenerateOperations(true);
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

        public IDiffManyConfiguration GenerateOperations(Config.Operations operationsToGenerate = Config.Operations.All)
        {
            if (operationsToGenerate != Config.Operations.None)
                Configuration.SetGenerateOperations(operationsToGenerate);
            return this;
        }
    }
}
