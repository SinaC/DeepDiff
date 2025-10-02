using System;
using System.Collections.Generic;

namespace DeepDiff
{
    /// <summary>
    /// Defines a listener which will be called on every insert/update/delete detection.
    /// </summary>
    public interface IOperationListener
    {
        /// <summary>
        /// Called when an insert is detected.
        /// </summary>
        /// <param name="entityTypeName">entity name</param>
        /// <param name="getKeysFunc">function to retrieve entity key(s). Will be returned as a dictionary(key name, key value)</param>
        /// <param name="getNavigationParentKeysFunc">function to retrieve navigation parent(s) key(s). Will be returned as a dictionary(parent entity name, dictionary(key name, key value))</param>
        void OnInsert(string entityTypeName, Func<Dictionary<string, object?>?> getKeysFunc, Func<Dictionary<string, Dictionary<string, object?>?>> getNavigationParentKeysFunc);

        /// <summary>
        /// Called when a delete is detected.
        /// </summary>
        /// <param name="entityTypeName">entity name</param>
        /// <param name="getKeysFunc">function to retrieve entity key(s). Will be returned as a dictionary(key name, key value)</param>
        /// <param name="getNavigationParentKeysFunc">function to retrieve navigation parent(s) key(s). Will be returned as a dictionary(parent entity name, dictionary(key name, key value))</param>
        void OnDelete(string entityTypeName, Func<Dictionary<string, object?>?> getKeysFunc, Func<Dictionary<string, Dictionary<string, object?>?>> getNavigationParentKeysFunc);

        /// <summary>
        /// Called when an update is detected.
        /// </summary>
        /// <param name="entityTypeName">entity name</param>
        /// <param name="propertyName">property name</param>
        /// <param name="getKeysFunc">function to retrieve entity key(s). Will be returned as a dictionary(key name, key value)</param>
        /// <param name="getOriginalValueFunc">function to retrieve original property value</param>
        /// <param name="getNewValueFunc">function to retrieve new property value</param>
        /// <param name="getNavigationParentKeysFunc">function to retrieve navigation parent(s) key(s). Will be returned as a dictionary(parent entity name, dictionary(key name, key value))</param>
        void OnUpdate(string entityTypeName, string propertyName, Func<Dictionary<string, object?>?> getKeysFunc, Func<object?> getOriginalValueFunc, Func<object?> getNewValueFunc, Func<Dictionary<string, Dictionary<string, object?>?>> getNavigationParentKeysFunc);
    }
}
