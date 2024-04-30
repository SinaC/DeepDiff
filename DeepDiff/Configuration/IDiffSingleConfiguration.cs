namespace DeepDiff.Configuration
{
    public interface IDiffSingleConfiguration
    {
        IDiffSingleConfiguration UseHashtable(bool use = true);
        IDiffSingleConfiguration HashtableThreshold(int threshold = 15);
        IDiffSingleConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(bool force = false);
        IDiffSingleConfiguration GenerateOperations(bool generate = true);
        IDiffSingleConfiguration OnlyGenerateOperations(bool onlyGenerate = false);
        IDiffSingleConfiguration UsePrecompiledEqualityComparer(bool use = true);
    }
}
