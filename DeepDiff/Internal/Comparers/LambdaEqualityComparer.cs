using System;
using System.Collections.Generic;

namespace DeepDiff.Internal.Comparers
{
    internal sealed class LambdaEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        private readonly Func<T, T, bool> _func;

        public LambdaEqualityComparer(Func<T, T, bool> func)
        {
            _func = func;
        }

        public bool Equals(T x, T y)
        {
            return _func(x, y);
        }

        public int GetHashCode(T obj)
        {
            return 0; // force Equals
        }
    }
}