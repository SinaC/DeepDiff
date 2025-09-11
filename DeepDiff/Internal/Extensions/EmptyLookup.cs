using System;
using System.Linq;

namespace DeepDiff.Internal.Extensions
{
    internal static class EmptyLookup<TKey, TElement>
    {
        private static Lazy<ILookup<TKey, TElement>> Lazy { get; } = new Lazy<ILookup<TKey, TElement>>(() => Enumerable.Empty<TElement>().ToLookup(x => default(TKey)!));

        public static ILookup<TKey, TElement> Instance
        {
            get => Lazy.Value;
        }
    }
}
