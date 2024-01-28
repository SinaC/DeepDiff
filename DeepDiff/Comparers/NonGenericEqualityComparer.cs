using System;
using System.Collections;
using System.Collections.Generic;

namespace DeepDiff.Comparers
{
    internal class NonGenericEqualityComparer : IEqualityComparer
    {
        private Func<object, object, bool> Comparer { get; set; } = null!;
        private Func<object, int> Hasher { get; set; } = null!;

        public static NonGenericEqualityComparer Create<T>(IEqualityComparer<T> equalityComparerOfT)
        {
            NonGenericEqualityComparer comparer = new NonGenericEqualityComparer
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
