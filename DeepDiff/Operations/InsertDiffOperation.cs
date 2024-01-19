using System.Collections.Generic;

namespace DeepDiff.Operations
{
    public class InsertDiffOperation : DiffOperationBase
    {
        public IReadOnlyCollection<string> Keys { get; init; }
    }
}
