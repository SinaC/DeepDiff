using System.Collections.Generic;

namespace DeepDiff.Operations
{
    public abstract class DiffOperationBase
    {
        public IReadOnlyDictionary<string, string> Keys { get; init; }

        public string EntityName { get; init; }
    }
}
