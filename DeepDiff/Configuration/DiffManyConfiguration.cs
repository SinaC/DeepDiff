namespace DeepDiff.Configuration
{
    internal sealed class DiffManyConfiguration : DiffSingleOrManyConfiguration, IDiffManyConfiguration
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

        public IDiffManyConfiguration ForceOnUpdateEvenIfModificatiosnDetectedOnlyInNestedLevel()
        {
            OnUpdateEvenIfModificatiosnDetectedOnlyInNestedLevel = true;
            return this;
        }
    }
}
