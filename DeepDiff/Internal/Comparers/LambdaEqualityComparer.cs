using System;
using System.Collections.Generic;

namespace DeepDiff.Internal.Comparers
{
    internal sealed class LambdaEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        private Func<T?, T?, bool> CompareFunc { get; }

        public LambdaEqualityComparer(Func<T?, T?, bool> compareFunc)
        {
            CompareFunc = compareFunc;
        }

        public bool Equals(T? x, T? y)
        {
            return CompareFunc(x, y);
        }

        public int GetHashCode(T obj)
        {
            return 0; // force Equals
        }
    }
}