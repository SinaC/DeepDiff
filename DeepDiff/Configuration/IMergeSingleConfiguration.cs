namespace DeepDiff.Configuration
{
    public interface IMergeSingleConfiguration
    {
        /// <summary>
        /// Sets the minimum threshold for the number of items in a collection to use a hashtable for comparison.
        /// </summary>
        /// <param name="threshold"></param>
        /// <remarks>default value for <paramref name="threshold"/> is <c>15</c></remarks>
        /// <returns></returns>
        IMergeSingleConfiguration HashtableThreshold(int threshold = 15);
        /// <summary>
        /// Indicates whether to force an update when modifications are detected only in the nested level.
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        IMergeSingleConfiguration ForceOnUpdateWhenModificationsDetectedOnlyInNestedLevel(bool force = false);
        /// <summary>
        /// Checks for duplicate keys in entities or nested entities.
        /// </summary>
        /// <param name="checkDuplicateKeys"></param>
        /// <remarks>default value for <paramref name="use"/> is <c>true</c></remarks>
        /// <returns></returns>
        IMergeSingleConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true);
    }
}
