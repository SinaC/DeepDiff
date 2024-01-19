using DeepDiff.Operations;
using System.Collections.Generic;

namespace DeepDiff
{
    public class DiffSingleResult<TEntity>
    {
        public TEntity Entity { get; init; }
        public IReadOnlyCollection<DiffOperationBase> Operations { get; init; }
        public long[] Elapsed { get; init; }
    }
}
