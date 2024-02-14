namespace DeepDiff.Configuration
{
    internal sealed class DiffManyConfiguration : DiffConfigurationBase, IDiffManyConfiguration
    {
        public IDiffManyConfiguration DisableHashTable()
        {
            UseHashtable = false;
            return this;
        }

        public IDiffManyConfiguration SetHashtableThreshold(int threshold)
        {
            HashtableThreshold = threshold;
            return this;
        }

        public IDiffManyConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel()
        {
            OnUpdateEvenIfModificationsDetectedOnlyInNestedLevel = true;
            return this;
        }

        public IDiffManyConfiguration DisableOperationsGeneration()
        {
            GenerateOperations = false;
            return this;
        }

        public IDiffManyConfiguration DisablePrecompiledEqualityComparer()
        {
            UsePrecompiledEqualityComparer = false;
            return this;
        }

        public IDiffManyConfiguration DisablePropagateNavigationKeyOnPureInsert()
        {
            PropagateNavigationKeyOnPureInsert = false;
            return this;
        }
    }
}
