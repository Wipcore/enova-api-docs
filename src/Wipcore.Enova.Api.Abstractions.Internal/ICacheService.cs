using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    /// <summary>
    /// Handles caching of queries.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Get cache entry for a request. Key is built on all in parameters.
        /// </summary>
        IEnumerable<IDictionary<string, object>> GetCache(IContextModel requestContext, IQueryModel query, Type type, BaseObjectList candidates = null);

        /// <summary>
        /// Get all cache keys for a type. Null to get all.
        /// </summary>
        IEnumerable<string> GetCacheKeys(string typeName = null);

        /// <summary>
        /// Set cache entry for a request. Key is built on all in parameters.
        /// </summary>
        void SetCache(IEnumerable<IDictionary<string, object>> value, IContextModel requestContext, IQueryModel query, Type type, BaseObjectList candidates = null);

        /// <summary>
        /// Clear all cache entries with the given type.
        /// </summary>
        void ClearCache(Type type = null);

        /// <summary>
        /// Clear all cache entries with the given type.
        /// </summary>
        void ClearCache(string typeName = null);

        /// <summary>
        /// Remove cache entry by key.
        /// </summary>
        void RemoveFromCache(string key);
    }
}