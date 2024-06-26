﻿namespace DeepDiff.Configuration
{
    public interface IMergeSingleConfiguration
    {
        IMergeSingleConfiguration UseHashtable(bool use = true);
        IMergeSingleConfiguration HashtableThreshold(int threshold = 15);
        IMergeSingleConfiguration ForceOnUpdateEvenIfModificationsDetectedOnlyInNestedLevel(bool force = false);
        IMergeSingleConfiguration GenerateOperations(bool generate = true);
        IMergeSingleConfiguration UsePrecompiledEqualityComparer(bool use = true);
    }
}
