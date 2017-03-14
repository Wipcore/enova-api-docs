using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
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
        /// Set cache entry for a request. Key is built on all in parameters.
        /// </summary>
        void SetCache(IEnumerable<IDictionary<string, object>> value, IContextModel requestContext, IQueryModel query, Type type, BaseObjectList candidates = null);

        /// <summary>
        /// Clear all cache entries with the given type.
        /// </summary>
        void ClearCache(Type type);
    }
}