namespace DeepDiff.Configuration
{
    public interface ICompareManyConfiguration
    {
        ICompareManyConfiguration UseHashtable(bool use = true);
        ICompareManyConfiguration HashtableThreshold(int threshold = 15);
        ICompareManyConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled);
    }
}
