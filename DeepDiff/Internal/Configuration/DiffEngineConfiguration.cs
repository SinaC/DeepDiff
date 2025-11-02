namespace DeepDiff.Internal.Configuration
{
    internal sealed class DiffEngineConfiguration
    {
        public int HashtableThreshold { get; private set; } = 15;
        public bool ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel { get; private set; }
        public bool CompareOnly { get; private set; }
        public bool CheckDuplicateKeys { get; private set; } = true;

        public void SetHashtableThreshold(int hashtableThreshold)
        {
            HashtableThreshold = hashtableThreshold;
        }

        public void SetForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(bool forceOnUpdateWhenModificationsDetectedOnlyInNestedLevel)
        {
            ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel = forceOnUpdateWhenModificationsDetectedOnlyInNestedLevel;
        }

        public void SetCompareOnly(bool compareOnly)
        {
            CompareOnly = compareOnly;
        }

        public void SetCheckDuplicateKeys(bool checkDuplicateKeys)
        {
            CheckDuplicateKeys = checkDuplicateKeys;
        }
    }
}
