namespace DeepDiff.Configuration
{
    public interface IDiffSingleConfiguration
    {
        IDiffSingleConfiguration DisableHashTable();
        IDiffSingleConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel();
    }
}
