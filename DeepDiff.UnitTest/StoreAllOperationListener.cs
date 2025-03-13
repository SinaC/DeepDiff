using DeepDiff.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DeepDiff.UnitTest
{
    internal class StoreAllOperationListener : IOperationListener
    {
        public ConcurrentBag<DiffOperationBase> Operations { get; } = new();

        public void OnInsert(string entityName, Func<Dictionary<string, object>> getKeysFunc)
        {
            Operations.Add(new InsertDiffOperation
            {
                EntityName = entityName,
                Keys = getKeysFunc().ToDictionary(x => x.Key, x => x.Value?.ToString())
            });
        }

        public void OnDelete(string entityName, Func<Dictionary<string, object>> getKeysFunc)
        {
            Operations.Add(new DeleteDiffOperation
            {
                EntityName = entityName,
                Keys = getKeysFunc().ToDictionary(x => x.Key, x => x.Value?.ToString())
            });
        }

        public void OnUpdate(string entityName, string propertyName, Func<Dictionary<string, object>> getKeysFunc, Func<object> getOriginalValueFunc, Func<object> getNewValueFunc)
        {
            Operations.Add(new UpdateDiffOperation
            {
                EntityName = entityName,
                Keys = getKeysFunc().ToDictionary(x => x.Key, x => x.Value?.ToString()),
                UpdatedProperties = new List<UpdateDiffOperationPropertyInfo>
                {
                    new UpdateDiffOperationPropertyInfo
                    {
                        PropertyName = propertyName,
                        ExistingValue = getOriginalValueFunc()?.ToString(),
                        NewValue = getNewValueFunc()?.ToString()
                    }
                }
            });
        }
    }

    internal abstract class DiffOperationBase
    {
        public Dictionary<string, string?> Keys { get; init; } = null!;

        public string EntityName { get; init; } = null!;
    }

    internal class InsertDiffOperation : DiffOperationBase
    {
    }

    internal class DeleteDiffOperation : DiffOperationBase
    {
    }

    internal class UpdateDiffOperation : DiffOperationBase
    {
        public List<UpdateDiffOperationPropertyInfo> UpdatedProperties { get; init; } = null!;
    }

    internal class UpdateDiffOperationPropertyInfo
    {
        public string PropertyName { get; init; } = null!;
        public string? ExistingValue { get; init; }
        public string? NewValue { get; init; }
    }
}
