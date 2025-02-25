using System.Collections.Generic;
using System.Linq;

namespace DeepDiff.Internal.Extensions
{
    internal static class IEnumerableExtensions
    {
        public static IEnumerable<T> FindDuplicate<T>(this IEnumerable<T> collection)
            => collection
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(x => x.Key);
    }
}
