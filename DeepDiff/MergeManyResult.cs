using DeepDiff.Operations;
using System.Collections.Generic;

namespace DeepDiff
{
    public readonly struct MergeManyResult<TEntity>
    {
        public IEnumerable<TEntity> Entities { get; init; }
        public IReadOnlyCollection<DiffOperationBase> Operations { get; init; }
    }
}
