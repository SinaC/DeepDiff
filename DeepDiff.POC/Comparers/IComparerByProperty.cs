using System.Collections;
using System.Collections.Generic;

namespace DeepDiff.POC.Comparers
{
    internal interface IComparerByProperty : IEqualityComparer, IEqualityComparer<object>
    {
        CompareByPropertyResult Compare(object? left, object? right);
    }
}
