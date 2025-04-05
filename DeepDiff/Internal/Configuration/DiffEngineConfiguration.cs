using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class DiffEngineConfiguration
    {
        public bool UseHashtable { get; private set; } = true;
        public int HashtableThreshold { get; private set; } = 15;
        public bool ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel { get; private set; }
        public bool CompareOnly { get; private set; }
        public EqualityComparers EqualityComparer { get; private set; } = EqualityComparers.Precompiled;

        public void SetUseHashtable(bool useHashtable)
        {
            UseHashtable = useHashtable;
        }

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

        public void SetEqualityComparer(EqualityComparers equalityComparer)
        {
            EqualityComparer = equalityComparer;
        }
    }
}
