using DeepDiff.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepDiff.Extensions
{
    internal static class DynamicEqualityComparerLinqIntegration
    {
        public static bool SequenceEqual<TSource>(
            this IEnumerable<TSource> source, IEnumerable<TSource> other, Func<TSource, TSource, bool> func)
            where TSource : class
        {
            return source.SequenceEqual(other, new LambdaEqualityComparer<TSource>(func));
        }
    }
}