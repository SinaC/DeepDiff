namespace DeepDiff.Configuration
{
    public interface IDiffManyConfiguration
    {
        IDiffManyConfiguration DisableHashTable();
        IDiffManyConfiguration ForceOnUpdateEvenIfModificatiosnDetectedOnlyInNestedLevel();
    }
}
