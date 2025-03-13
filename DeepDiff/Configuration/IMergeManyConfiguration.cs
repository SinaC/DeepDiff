namespace DeepDiff.Configuration
{
    public interface IMergeManyConfiguration
    {
        IMergeManyConfiguration UseHashtable(bool use = true);
        IMergeManyConfiguration HashtableThreshold(int threshold = 15);
        IMergeManyConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled);
        IMergeManyConfiguration UseParallelism(bool use = false);
        IMergeManyConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(bool force = false);
    }
}
