namespace DeepDiff.Configuration
{
    /// <summary>
    /// Interface for configuring the comparison of a single entity.
    /// </summary>
    public interface ICompareSingleConfiguration
    {
        /// <summary>
        /// Sets the minimum threshold for the number of items in a collection to use a hashtable for comparison.
        /// </summary>
        /// <param name="threshold"></param>
        /// <remarks>default value for <paramref name="threshold"/> is <c>15</c></remarks>
        /// <returns></returns>
        ICompareSingleConfiguration HashtableThreshold(int threshold = 15);
        /// <summary>
        /// Indicates whether to check for duplicate keys in entities or nested entities.
        /// </summary>
        /// <param name="checkDuplicateKeys"></param>
        /// <remarks>default value for <paramref name="use"/> is <c>true</c></remarks>
        /// <returns></returns>
        ICompareSingleConfiguration SetCheckDuplicateKeys(bool checkDuplicateKeys = true);
    }
}
