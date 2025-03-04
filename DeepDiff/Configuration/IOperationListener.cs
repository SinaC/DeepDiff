using System;
using System.Collections.Generic;

namespace DeepDiff.Configuration
{
    public interface IOperationListener
    {
        void OnInsert(string entityName, Func<Dictionary<string, object>> getKeysFunc);
        void OnDelete(string entityName, Func<Dictionary<string, object>> getKeysFunc);
        void OnUpdate(string entityName, string propertyName, Func<Dictionary<string, object>> getKeysFunc, Func<object> getOriginalValueFunc, Func<object> getNewValueFunc);
    }
}
