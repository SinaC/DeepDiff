using Config = DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class DiffEngineConfiguration
    {
        public bool UseHashtable { get; private set; } = true;
        public int HashtableThreshold { get; private set; } = 15;
        public bool ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel { get; private set; }
        public Config.DiffOperations OperationsToGenerate { get; private set; } = Config.DiffOperations.None;
        public bool GenerateOperationsOnly { get; private set; }
        public bool UsePrecompiledEqualityComparer { get; private set; } = true;

        public void SetUseHashtable(bool useHashtable)
        {
            UseHashtable = useHashtable;
        }

        public void SetHashtableThreshold(int hashtableThreshold)
        {
            HashtableThreshold = hashtableThreshold;
        }

        public void SetForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(bool forceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel)
        {
            ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel = forceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel;
        }

        public void SetGenerateOperations(Config.DiffOperations operationsToGenerate)
        {
            OperationsToGenerate = operationsToGenerate;
        }

        public void SetGenerateOperationsOnly(bool generateOperationsOnly)
        {
            GenerateOperationsOnly = generateOperationsOnly;
        }

        public void SetUsePrecompiledEqualityComparer(bool usePrecompiledEqualityComparer)
        {
            UsePrecompiledEqualityComparer = usePrecompiledEqualityComparer;
        }
    }
}
