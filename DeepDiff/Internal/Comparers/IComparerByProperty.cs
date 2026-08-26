using System.Collections;

namespace DeepDiff.Internal.Comparers;

internal interface IComparerByProperty : IEqualityComparer, IEqualityComparer<object>
{
    CompareByPropertyResult Compare(object? left, object? right);
}
