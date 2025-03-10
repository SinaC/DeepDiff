﻿namespace DeepDiff.Configuration
{
    public interface IMergeManyConfiguration
    {
        IMergeManyConfiguration UseHashtable(bool use = true);
        IMergeManyConfiguration HashtableThreshold(int threshold = 15);
        IMergeManyConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(bool force = false);
        IMergeManyConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled);
    }
}
