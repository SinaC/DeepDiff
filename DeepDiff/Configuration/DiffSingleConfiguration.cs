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

        public IDiffSingleConfiguration ForceOnUpdateEvenIfModificatiosnDetectedOnlyInNestedLevel()
        {
            OnUpdateEvenIfModificatiosnDetectedOnlyInNestedLevel = true;
            return this;
        }
    }
}
