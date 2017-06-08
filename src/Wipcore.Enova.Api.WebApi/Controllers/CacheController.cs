using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.OAuth;

namespace Wipcore.eNova.Api.WebApi.Controllers
{
    /// <summary>
    /// This controller is used to view and remove cache entries.
    /// </summary>
    [Route("api/[controller]")]
    public class CacheController : EnovaApiController
    {
        private readonly ICacheService _cacheService;

        public CacheController(IExceptionService exceptionService, ICacheService cacheService) : base(exceptionService)
        {
            _cacheService = cacheService;
        }


        /// <summary>
        /// Get cache keys.
        /// </summary>
        /// <param name="enovaTypeName">The type for which to get keys. Null to get all.</param>
        [HttpGet]
        [HttpGet("{resource}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public IEnumerable<string> GetAll(string enovaTypeName = null)
        {
            return _cacheService.GetCacheKeys(enovaTypeName);
        }

        /// <summary>
        /// Remove cache entries by type.
        /// </summary>
        /// <param name="enovaTypeName">The type for which to remove. Null to remove all.</param>
        [HttpDelete]
        [HttpDelete("{resource}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Remove(string enovaTypeName = null)
        {
            _cacheService.ClearCache(enovaTypeName);
        }

        /// <summary>
        /// Remove cache entry by key.
        /// </summary>
        /// <param name="key">The type for which to remove. Null to remove all.</param>
        [HttpDelete("key-{key}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void RemoveByKey(string key = null)
        {
            _cacheService.RemoveFromCache(key);
        }
    }
}
