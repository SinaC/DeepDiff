using System.Collections.Generic;

namespace DeepDiff.Operations
{
    public class DeleteDiffOperation : DiffOperationBase
    {
        public IReadOnlyCollection<string> Keys { get; init; }
    }
}
