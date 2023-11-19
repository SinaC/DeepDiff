using EntityMerger.Comparer;

namespace EntityMerger.Extensions;

internal static class DynamicEqualityComparerLinqIntegration
{
    public static bool SequenceEqual<TSource>(
        this IEnumerable<TSource> source, IEnumerable<TSource> other, Func<TSource, TSource, bool> func)
        where TSource : class
    {
        return source.SequenceEqual(other, new DynamicEqualityComparer<TSource>(func));
    }
}
