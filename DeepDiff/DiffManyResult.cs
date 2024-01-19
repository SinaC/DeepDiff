using DeepDiff.Operations;
using System.Collections.Generic;

namespace DeepDiff
{
    public class DiffManyResult<TEntity>
    {
        public IEnumerable<TEntity> Entities { get; init; }
        public IReadOnlyCollection<DiffOperationBase> Operations { get; init; }
        public long[] Elapsed { get; init; }
    }
}
