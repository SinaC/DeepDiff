using DeepDiff.Internal.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepDiff.Internal.Extensions
{
    internal static class IEnumerableExtensions
    {
        public static bool SequenceEqual<TSource>(
           this IEnumerable<TSource> source, IEnumerable<TSource> other, Func<TSource?, TSource?, bool> func)
           where TSource : class
            => source.SequenceEqual(other, new LambdaEqualityComparer<TSource>(func));

        public static IEnumerable<T> FindDuplicate<T>(this IEnumerable<T> collection)
            => collection
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(x => x.Key);
    }
}
