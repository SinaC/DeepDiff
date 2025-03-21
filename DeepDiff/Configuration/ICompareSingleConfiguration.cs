﻿using DeepDiff.Internal.Configuration;

namespace DeepDiff.Configuration
{
    public interface ICompareSingleConfiguration
    {
        ICompareSingleConfiguration UseHashtable(bool use = true);
        ICompareSingleConfiguration HashtableThreshold(int threshold = 15);
        ICompareSingleConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled);
        ICompareSingleConfiguration UseParallelism(bool use = false);
    }
}
