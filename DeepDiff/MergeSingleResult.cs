using DeepDiff.Operations;
using System.Collections.Generic;

namespace DeepDiff
{
    public readonly struct MergeSingleResult<TEntity>
    {
        public TEntity Entity { get; init; }
        public IReadOnlyCollection<DiffOperationBase> Operations { get; init; }
    }
}
