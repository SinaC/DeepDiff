using System.Collections;
using System.Collections.Generic;

namespace DeepDiff.Internal.Comparers
{
    internal interface IComparerByProperty : IEqualityComparer, IEqualityComparer<object>
    {
        CompareByPropertyResult Compare(object? left, object? right);
    }
}
