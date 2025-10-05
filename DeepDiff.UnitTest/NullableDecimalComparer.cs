using System.Diagnostics.CodeAnalysis;

namespace DeepDiff.UnitTest;

internal class NullableDecimalComparer : IEqualityComparer<decimal?>
{
    private int Decimals { get; }
    private decimal Modulus { get; }

    public NullableDecimalComparer(int decimals)
    {
        Decimals = decimals;
        Modulus = 1m / (decimal)Math.Pow(10, Decimals);
    }

    public bool Equals(decimal? left, decimal? right)
    {
        if (left == null && right == null)
            return true;
        if (left == null && right != null)
            return false;
        if (left != null && right == null)
            return false;
        return EqualsTruncated(left!.Value, right!.Value);
    }

    public int GetHashCode([DisallowNull] decimal? d)
        => d?.GetHashCode() ?? 0;

    private bool EqualsTruncated(decimal left, decimal right)
    {
        var leftTruncated = left - (left % Modulus);
        var rightTruncated = right - (right % Modulus);
        return leftTruncated == rightTruncated;
    }
}
