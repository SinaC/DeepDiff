namespace DeepDiff.Configuration
{
    internal sealed class DiffSingleConfiguration : DiffSingleOrManyConfiguration, IDiffSingleConfiguration
    {
        public IDiffSingleConfiguration DisableHashTable()
        {
            UseHashtable = false;
            return this;
        }

        public IDiffSingleConfiguration SetHashtableThreshold(int threshold)
        {
            HashtableThreshold = threshold;
            return this;
        }

        public IDiffSingleConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel()
        {
            OnUpdateEvenIfModificationsDetectedOnlyInNestedLevel = true;
            return this;
        }

        public IDiffSingleConfiguration DisableOperationsGeneration()
        {
            GenerateOperations = false;
            return this;
        }

        public IDiffSingleConfiguration DisablePrecompiledEqualityComparer()
        {
            UsePrecompiledEqualityComparer = false;
            return this;
        }

        public IDiffSingleConfiguration SetDecimalPrecision(int precision)
        {
            DecimalPrecision = precision;
            return this;
        }
    }
}
