using System.Collections;
using System.Collections.Generic;

namespace DeepDiff.Comparers
{
    internal interface IComparerByProperty : IEqualityComparer
    {
        IEnumerable<ComparerByPropertyResult> Compare(object? x, object? y);
    }
}
