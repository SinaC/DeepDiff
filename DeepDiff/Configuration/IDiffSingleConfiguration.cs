namespace DeepDiff.Configuration
{
    public interface IDiffSingleConfiguration
    {
        IDiffSingleConfiguration UseHashtable(bool use = true);
        IDiffSingleConfiguration HashtableThreshold(int threshold = 15);
        IDiffSingleConfiguration UsePrecompiledEqualityComparer(bool use = true);
        IDiffSingleConfiguration GenerateOperations(Operations operationsToGenerate = Operations.All);
    }
}
