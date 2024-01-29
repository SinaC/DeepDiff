using System.Collections.Generic;

namespace DeepDiff.Operations
{
    public class UpdateDiffOperation : DiffOperationBase
    {
        public IReadOnlyCollection<UpdateDiffOperationPropertyInfo> UpdatedProperties { get; init; }
        public IReadOnlyCollection<UpdateDiffOperationPropertyInfo> SetValueProperties { get; init; }
        public IReadOnlyCollection<UpdateDiffOperationPropertyInfo> CopyValuesProperties { get; init; }
    }
}
