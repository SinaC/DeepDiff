using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal class DiffEngineConfiguration
    {
        public bool UseHashtable { get; private set; } = true;
        public int HashtableThreshold { get; private set; } = 15;
        public bool ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel { get; private set; } = false;
        public GenerateOperations OperationsToGenerate { get; private set; } = GenerateOperations.All;
        public bool GenerateOperationsOnly { get; private set; } = false;
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

        public void SetGenerateOperations(bool generateOperations)
        {
            OperationsToGenerate = generateOperations
                ? GenerateOperations.All
                : GenerateOperations.None;
        }

        public void SetGenerateOperations(GenerateOperations operationsToGenerate)
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
