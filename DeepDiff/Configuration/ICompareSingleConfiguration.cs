namespace DeepDiff.Configuration
{
    public interface ICompareSingleConfiguration
    {
        ICompareSingleConfiguration UseHashtable(bool use = true);
        ICompareSingleConfiguration HashtableThreshold(int threshold = 15);
        ICompareSingleConfiguration UsePrecompiledEqualityComparer(bool use = true);
    }
}
