namespace DeepDiff.Configuration
{
    public interface IMergeManyConfiguration
    {
        /// <summary>
        /// Indicates whether to use a hashtable for comparison.
        /// </summary>
        /// <param name="use"></param>
        /// <remarks>default value for <paramref name="use"/> is <c>true</c></remarks>
        /// <returns></returns>
        IMergeManyConfiguration UseHashtable(bool use = true);
        /// <summary>
        /// Sets the minimum threshold for the number of items in a collection to use a hashtable for comparison.
        /// </summary>
        /// <param name="threshold"></param>
        /// <remarks>default value for <paramref name="threshold"/> is <c>15</c></remarks>
        /// <returns></returns>
        IMergeManyConfiguration HashtableThreshold(int threshold = 15);
        /// <summary>
        /// Sets the equality comparer to use for comparison.
        /// </summary>
        /// <param name="equalityComparer"></param>
        /// <remarks>default value for <paramref name="equalityComparer"/> is <c>EqualityComparers.Precompiled</c></remarks>
        /// <returns></returns>
        IMergeManyConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled);
        /// <summary>
        /// Indicates whether to force an update when modifications are detected only in the nested level.
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        IMergeManyConfiguration ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(bool force = false);
        /// <summary>
        /// Indicates whether to check for duplicate keys in entities or nested entities.
        /// </summary>
        /// <param name="checkDuplicateKeys"></param>
        /// <remarks>default value for <paramref name="use"/> is <c>true</c></remarks>
        /// <returns></returns>
        IMergeManyConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true);
    }
}
