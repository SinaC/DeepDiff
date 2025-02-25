namespace DeepDiff.Configuration
{
    public interface IMergeManyConfiguration
    {
        IMergeManyConfiguration UseHashtable(bool use = true);
        IMergeManyConfiguration HashtableThreshold(int threshold = 15);
        IMergeManyConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(bool force = false);
        IMergeManyConfiguration GenerateOperations(bool generate = true);

        IMergeManyConfiguration GenerateOperations(Operations operationsToGenerate = Operations.All);
        IMergeManyConfiguration UsePrecompiledEqualityComparer(bool use = true);
    }
}
