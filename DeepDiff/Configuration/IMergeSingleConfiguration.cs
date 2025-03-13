namespace DeepDiff.Configuration
{
    public interface IMergeSingleConfiguration
    {
        IMergeSingleConfiguration UseHashtable(bool use = true);
        IMergeSingleConfiguration HashtableThreshold(int threshold = 15);
        IMergeSingleConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled);
        IMergeSingleConfiguration UseParallelism(bool use = false);
        IMergeSingleConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(bool force = false);
    }
}
