namespace DeepDiff.Configuration
{
    public interface IDiffSingleConfiguration
    {
        IDiffSingleConfiguration DisableHashTable();
        IDiffSingleConfiguration SetHashtableThreshold(int threshold);
        IDiffSingleConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel();
        IDiffSingleConfiguration DisableOperationsGeneration();
    }
}
