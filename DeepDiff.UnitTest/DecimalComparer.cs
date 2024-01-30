using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DeepDiff.UnitTest
{
    internal class DecimalComparer : IEqualityComparer<decimal>
    {
        private int Decimals { get; }
        private decimal Modulus { get; }

        public DecimalComparer(int decimals)
        {
            Decimals = decimals;
            Modulus = 1m / (decimal)Math.Pow(10, Decimals);
        }

        public bool Equals(decimal left, decimal right)
            => EqualsTruncated(left, right);

        public int GetHashCode([DisallowNull] decimal d)
            => d.GetHashCode();

        private bool EqualsTruncated(decimal left, decimal right)
        {
            var leftTruncated = left - (left % Modulus);
            var rightTruncated = right - (right % Modulus);
            return leftTruncated == rightTruncated;
        }
    }
}
