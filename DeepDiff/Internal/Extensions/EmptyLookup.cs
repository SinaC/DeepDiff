﻿using System.Linq;

namespace DeepDiff.Internal.Extensions
{
    internal static class EmptyLookup<TKey, TElement>
    {
        private static ILookup<TKey, TElement> _instance { get; } = Enumerable.Empty<TElement>().ToLookup(x => default(TKey));

        public static ILookup<TKey, TElement> Instance
        {
            get => _instance;
        }
    }
}
