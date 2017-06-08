using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Library;

namespace Wipcore.eNova.Api.WebApi.Services
{
    /// <summary>
    /// Handles caching of queries.
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly IConfigurationRoot _configuration;
        private readonly ObjectCache _cache;
        private readonly IAuthService _authService;

        public CacheService(IConfigurationRoot configuration, ObjectCache cache, IAuthService authService)
        {
            _configuration = configuration;
            _cache = cache;
            _authService = authService;
        }


        /// <summary>
        /// Get cache entry for a request. Key is built on all in parameters.
        /// </summary>
        public IEnumerable<IDictionary<string, object>> GetCache(IContextModel requestContext, IQueryModel query, Type type, BaseObjectList candidates = null)
        {
            if (query.Cache == false)
                return null;

            if (query.Cache != true && !Convert.ToBoolean(_configuration["Cache:Enabled"] ?? "true"))
                return null;

            var key = Key(requestContext, query, type, candidates);

            return _cache.Get(key) as IEnumerable<IDictionary<string, object>>;
        }

        /// <summary>
        /// Get all cache keys for a type. Null to get all.
        /// </summary>
        public IEnumerable<string> GetCacheKeys(string typeName = null)
        {
            return _cache.Where(x => typeName == null || x.Key.StartsWith(typeName + "-")).Select(x => x.Key);
        }

        /// <summary>
        /// Set cache entry for a request. Key is built on all in parameters.
        /// </summary>
        public void SetCache(IEnumerable<IDictionary<string, object>> value, IContextModel requestContext, IQueryModel query, Type type, BaseObjectList candidates = null)
        {
            if (query.Cache == false)
                return;

            if (query.Cache != true && !Convert.ToBoolean(_configuration["Cache:Enabled"] ?? "true"))
                return;

            var cacheMinutes = Convert.ToInt32(_configuration["Cache:Minutes"]);
            var key = Key(requestContext, query, type, candidates);

            _cache.Set(key, value, DateTime.Now.AddMinutes(cacheMinutes));
        }

        /// <summary>
        /// Clear all cache entries with the given type. Null to clear all.
        /// </summary>
        public void ClearCache(Type type = null)
        {
            ClearCache(type?.Name);   
        }

        /// <summary>
        /// Clear all cache entries with the given type. Null to clear all.
        /// </summary>
        public void ClearCache(string typeName = null)
        {
            var keys = _cache.Where(x => typeName == null || x.Key.StartsWith(typeName + "-", StringComparison.CurrentCultureIgnoreCase)).Select(x => x.Key);
            keys.ForEach(x => _cache.Remove(x));
        }

        /// <summary>
        /// Remove cache entry by key.
        /// </summary>
        public void RemoveFromCache(string key)
        {
            _cache.Remove(key);
        }


        private string Key(IContextModel requestContext, IQueryModel query, Type type, BaseObjectList candidates = null)
        {
            var key = $"{type.Name}-{_authService.GetLoggedInAlias() ?? String.Empty}{requestContext.Currency}{requestContext.Language}{requestContext.Market}{query.Filter}{query.Page}{query.Properties}{query.Size}{query.Sort}{query.Template}";
            if (candidates != null)
                key += String.Join(String.Empty, candidates.Cast<BaseObject>().Select(x => x.ID.ToString()));

            return key;
        }
    }
}
