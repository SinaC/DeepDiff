namespace DeepDiff.Configuration
{
    public interface IMergeSingleConfiguration
    {
        /// <summary>
        /// Indicates whether to use a hashtable for comparison.
        /// </summary>
        /// <param name="use"></param>
        /// <remarks>default value for <paramref name="use"/> is <c>true</c></remarks>
        /// <returns></returns>
        IMergeSingleConfiguration UseHashtable(bool use = true);
        /// <summary>
        /// Sets the minimum threshold for the number of items in a collection to use a hashtable for comparison.
        /// </summary>
        /// <param name="threshold"></param>
        /// <remarks>default value for <paramref name="threshold"/> is <c>15</c></remarks>
        /// <returns></returns>
        IMergeSingleConfiguration HashtableThreshold(int threshold = 15);
        /// <summary>
        /// Sets the equality comparer to use for comparison.
        /// </summary>
        /// <param name="equalityComparer"></param>
        /// <remarks>default value for <paramref name="equalityComparer"/> is <c>EqualityComparers.Precompiled</c></remarks>
        /// <returns></returns>
        IMergeSingleConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled);
        /// <summary>
        /// Indicates whether to force an update when modifications are detected only in the nested level.
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        IMergeSingleConfiguration ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(bool force = false);
    }
}
