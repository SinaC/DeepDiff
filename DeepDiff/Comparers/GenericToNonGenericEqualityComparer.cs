using System;
using System.Collections;
using System.Collections.Generic;

namespace DeepDiff.Comparers
{
    internal class GenericToNonGenericEqualityComparer : IEqualityComparer
    {
        private Func<object, object, bool> Comparer { get; set; } = null!;
        private Func<object, int> Hasher { get; set; } = null!;

        public static GenericToNonGenericEqualityComparer Create<T>(IEqualityComparer<T> equalityComparerOfT)
        {
            GenericToNonGenericEqualityComparer comparer = new GenericToNonGenericEqualityComparer
            {
                Comparer = (left, right) => equalityComparerOfT.Equals((T)left, (T)right),
                Hasher = x => equalityComparerOfT.GetHashCode((T)x)
            };
            return comparer;
        }

        public new bool Equals(object? x, object? y)
            => Comparer(x, y);

        public int GetHashCode(object obj)
            => Hasher(obj);
    }
}
