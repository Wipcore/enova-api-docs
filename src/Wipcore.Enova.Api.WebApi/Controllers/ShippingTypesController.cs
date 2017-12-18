using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ShippingTypesController : EnovaApiController
    {
        private readonly IObjectService _objectService;

        public ShippingTypesController(IExceptionService exceptionService, IObjectService objectService)
            : base(exceptionService)
        {
            _objectService = objectService;
        }

        [HttpHead("{identifier}")]
        [AllowAnonymous]
        public void Head([FromUri]string identifier)
        {
            var found = _objectService.Exists<EnovaShippingType>(identifier);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        [HttpHead("id-{id}")]
        [AllowAnonymous]
        public void Head([FromUri]int id)
        {
            var found = _objectService.Exists<EnovaShippingType>(id);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Get a list of shippingtypes.
        /// </summary>
        [HttpGet()]
        [AllowAnonymous]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.GetMany<EnovaShippingType>(requestContext, query);
        }

        /// <summary>
        /// Get a shippingtype specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        [AllowAnonymous]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaShippingType>(requestContext, query, identifier);
        }

        /// <summary>
        /// Get a shippingtype specified by id. 
        /// </summary>
        [HttpGet("id-{id}")]
        [AllowAnonymous]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, int id)
        {
            return _objectService.Get<EnovaShippingType>(requestContext, query, id);
        }

        /// <summary>
        /// Get EnovaShippingType specified by ids. 
        /// </summary>
        [HttpGet("ids")]
        [AllowAnonymous]
        public IEnumerable<IDictionary<string, object>> GetManyIds([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string ids)
        {
            var listIds = ids.Split(',').Select(x => Convert.ToInt32(x.Trim())).Distinct();
            return _objectService.GetMany<EnovaShippingType>(requestContext, query, listIds);
        }

        /// <summary>
        /// Get EnovaShippingType specified by identifiers. 
        /// </summary>
        [HttpGet("identifiers")]
        [AllowAnonymous]
        public IEnumerable<IDictionary<string, object>> GetManyIdentifiers([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string identifiers)
        {
            var listIdentifiers = identifiers.Split(',').Select(x => x.Trim()).Distinct();
            return _objectService.GetMany<EnovaShippingType>(requestContext, query, listIdentifiers);
        }

        /// <summary>
        /// Create or update a shippingtype.
        /// </summary>
        [HttpPut()]
        [Authorize(Roles = AuthService.AdminRole)]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _objectService.Save<EnovaShippingType>(requestContext, values);
        }

        /// <summary>
        /// Delete a shippingtype.
        /// </summary>
        [HttpDelete("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(int id)
        {
            var success = _objectService.Delete<EnovaShippingType>(id);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Delete a shippingtype.
        /// </summary>
        [HttpDelete("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(string identifier)
        {
            var success = _objectService.Delete<EnovaShippingType>(identifier);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Add a dynamic property to a shippingtype.
        /// </summary>
        [HttpPost("AddProperty")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void AddProperty(string propertyName, BaseObject.PropertyType propertyType, bool languageDependant = false, int maxStringLength = 255)
        {
            _objectService.AddProperty<EnovaShippingType>(propertyName, propertyType, languageDependant, maxStringLength);
        }

        /// <summary>
        /// Remove a dynamic property from a shippingtype.
        /// </summary>
        [HttpPost("RemoveProperty")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void RemoveProperty(string propertyName)
        {
            _objectService.RemoveProperty<EnovaShippingType>(propertyName);
        }
    }
}
