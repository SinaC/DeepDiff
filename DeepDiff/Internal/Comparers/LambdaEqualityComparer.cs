namespace DeepDiff.Internal.Comparers;

internal sealed class LambdaEqualityComparer<T>(Func<T?, T?, bool> compareFunc) : IEqualityComparer<T>
    where T : class
{
    private Func<T?, T?, bool> CompareFunc { get; } = compareFunc;

    public bool Equals(T? x, T? y)
    {
        return CompareFunc(x, y);
    }

    public int GetHashCode(T obj)
    {
        return 0; // force Equals
    }
}