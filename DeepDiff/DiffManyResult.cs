using DeepDiff.Operations;
using System.Collections.Generic;

namespace DeepDiff
{
    public readonly struct DiffManyResult<TEntity>
    {
        public IEnumerable<TEntity> Entities { get; init; }
        public IReadOnlyCollection<DiffOperationBase> Operations { get; init; }
    }
}
