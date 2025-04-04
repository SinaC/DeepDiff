using DeepDiff.Internal.Configuration;

namespace DeepDiff.Configuration
{
    /// <summary>
    /// Interface for configuring the comparison of multiple entities.
    /// </summary>
    public interface ICompareManyConfiguration
    {
        /// <summary>
        /// Indicates whether to use a hashtable for comparison.
        /// </summary>
        /// <param name="use"></param>
        /// <remarks>default value for <paramref name="use"/> is <c>true</c></remarks>
        /// <returns></returns>
        ICompareManyConfiguration UseHashtable(bool use = true);
        /// <summary>
        /// Sets the minimum threshold for the number of items in a collection to use a hashtable for comparison.
        /// </summary>
        /// <param name="threshold"></param>
        /// <remarks>default value for <paramref name="threshold"/> is <c>15</c></remarks>
        /// <returns></returns>
        ICompareManyConfiguration HashtableThreshold(int threshold = 15);
        /// <summary>
        /// Sets the equality comparer to use for comparison.
        /// </summary>
        /// <param name="equalityComparer"></param>
        /// <remarks>default value for <paramref name="equalityComparer"/> is <c>EqualityComparers.Precompiled</c></remarks>
        /// <returns></returns>
        ICompareManyConfiguration SetEqualityComparer(EqualityComparers equalityComparer = EqualityComparers.Precompiled);
        /// <summary>
        /// Indicates whether to use parallelism
        /// </summary>
        /// <param name="use"></param>
        /// <remarks>default value for <paramref name="use"/> is <c>false</c></remarks>
        /// <returns></returns>
        ICompareManyConfiguration UseParallelism(bool use = false);
    }
}
