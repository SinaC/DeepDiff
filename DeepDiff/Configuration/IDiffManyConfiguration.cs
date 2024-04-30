namespace DeepDiff.Configuration
{
    public interface IDiffManyConfiguration
    {
        IDiffManyConfiguration UseHashtable(bool use = true);
        IDiffManyConfiguration HashtableThreshold(int threshold = 15);
        IDiffManyConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(bool force = false);
        IDiffManyConfiguration GenerateOperations(bool generate = true);
        IDiffManyConfiguration OnlyGenerateOperations(bool onlyGenerate = false);
        IDiffManyConfiguration UsePrecompiledEqualityComparer(bool use = true);
    }
}
