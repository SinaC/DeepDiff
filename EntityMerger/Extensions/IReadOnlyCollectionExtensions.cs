namespace EntityMerger.Extensions
{
    internal static class IReadOnlyCollectionExtensions
    {
        public static bool HasDuplicate<T>(this IReadOnlyCollection<T> collection)
            => collection.Count != collection.Distinct().Count();

        public static IEnumerable<T> FindDuplicate<T>(this IReadOnlyCollection<T> collection)
            => collection
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(x => x.Key);
    }
}
