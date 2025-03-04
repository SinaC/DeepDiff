using DeepDiff.Configuration;

namespace DeepDiff.Internal.Configuration
{
    internal sealed class DiffEngineConfiguration
    {
        public bool UseHashtable { get; private set; } = true;
        public int HashtableThreshold { get; private set; } = 15;
        public bool ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel { get; private set; }
        public bool CompareOnly { get; private set; }
        public bool UsePrecompiledEqualityComparer { get; private set; } = true;

        public void SetUseHashtable(bool useHashtable)
        {
            UseHashtable = useHashtable;
        }

        public void SetHashtableThreshold(int hashtableThreshold)
        {
            HashtableThreshold = hashtableThreshold;
        }

        public void SetForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(bool forceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel)
        {
            ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel = forceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel;
        }

        public void SetCompareOnly(bool compareOnly)
        {
            CompareOnly = compareOnly;
        }

        public void SetUsePrecompiledEqualityComparer(bool usePrecompiledEqualityComparer)
        {
            UsePrecompiledEqualityComparer = usePrecompiledEqualityComparer;
        }
    }
}
