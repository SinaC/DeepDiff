namespace DeepDiff.Configuration
{
    public interface IDiffManyConfiguration
    {
        IDiffManyConfiguration DisableHashTable();
        IDiffManyConfiguration SetHashtableThreshold(int threshold);
        IDiffManyConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel();
        IDiffManyConfiguration DisableOperationsGeneration();
        IDiffManyConfiguration DisablePrecompiledEqualityComparer();
        IDiffManyConfiguration SetDecimalPrecision(int precision);
    }
}
