﻿using Config = DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal class DiffEngineConfiguration
    {
        public bool UseHashtable { get; private set; } = true;
        public int HashtableThreshold { get; private set; } = 15;
        public bool ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel { get; private set; } = false;
        public Config.Operations OperationsToGenerate { get; private set; } = Config.Operations.All;
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
                ? Config.Operations.All
                : Config.Operations.None;
        }

        public void SetGenerateOperations(Config.Operations operationsToGenerate)
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
