namespace DeepDiff.Configuration
{
    public interface IDiffManyConfiguration
    {
        IDiffManyConfiguration UseHashtable(bool use = true);
        IDiffManyConfiguration HashtableThreshold(int threshold = 15);
        IDiffManyConfiguration UsePrecompiledEqualityComparer(bool use = true);
        IDiffManyConfiguration GenerateOperations(Operations operationsToGenerate = Operations.All);
    }
}
