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
    public class AttributeTypesController : EnovaApiController
    {
        private readonly IObjectService _objectService;

        public AttributeTypesController(IExceptionService exceptionService, IObjectService objectService)
            :base (exceptionService)
        {
            _objectService = objectService;
        }

        /// <summary>
        /// Check if the attributetype exists.
        /// </summary>
        [HttpHead("{identifier}")]
        [AllowAnonymous]
        public void Head([FromUri]string identifier)
        {
            var found = _objectService.Exists<EnovaAttributeType>(identifier);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Check if the attributetype exists.
        /// </summary>
        [HttpHead("id-{id}")]
        [AllowAnonymous]
        public void Head([FromUri]int id)
        {
            var found = _objectService.Exists<EnovaAttributeType>(id);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Get a list of attribute types.
        /// </summary>
        [HttpGet()]
        [AllowAnonymous]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.GetMany<EnovaAttributeType>(requestContext, query);
        }

        /// <summary>
        /// Get an attribute type specified by identifier.
        /// </summary>
        [HttpGet("{identifier}")]
        [AllowAnonymous]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaAttributeType>(requestContext, query, identifier);
        }

        /// <summary>
        /// Get an attribute type specified by id.
        /// </summary>
        [HttpGet("id-{id}")]
        [AllowAnonymous]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, int id)
        {
            return _objectService.Get<EnovaAttributeType>(requestContext, query, id);
        }

        /// <summary>
        /// Get EnovaAttributeType specified by ids. 
        /// </summary>
        [HttpGet("ids")]
        [AllowAnonymous]
        public IEnumerable<IDictionary<string, object>> GetManyIds([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string ids)
        {
            var listIds = ids.Split(',').Select(x => Convert.ToInt32(x.Trim())).Distinct();
            return _objectService.GetMany<EnovaAttributeType>(requestContext, query, listIds);
        }

        /// <summary>
        /// Get EnovaAttributeType specified by identifiers. 
        /// </summary>
        [HttpGet("identifiers")]
        [AllowAnonymous]
        public IEnumerable<IDictionary<string, object>> GetManyIdentifiers([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string identifiers)
        {
            var listIdentifiers = identifiers.Split(',').Select(x => x.Trim()).Distinct();
            return _objectService.GetMany<EnovaAttributeType>(requestContext, query, listIdentifiers);
        }

        /// <summary>
        /// Create or update an attributetype.
        /// </summary>
        [HttpPut()]
        [Authorize(Roles = AuthService.AdminRole)]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _objectService.Save<EnovaAttributeType>(requestContext, values);
        }

        /// <summary>
        /// Delete a attributetype.
        /// </summary>
        [HttpDelete("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(int id)
        {
            var success = _objectService.Delete<EnovaAttributeType>(id);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Delete a attributetype.
        /// </summary>
        [HttpDelete("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(string identifier)
        {
            var success = _objectService.Delete<EnovaAttributeType>(identifier);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Add a dynamic property to a attributetype.
        /// </summary>
        [HttpPost("AddProperty")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void AddProperty(string propertyName, BaseObject.PropertyType propertyType, bool languageDependant = false, int maxStringLength = 255)
        {
            _objectService.AddProperty<EnovaAttributeType>(propertyName, propertyType, languageDependant, maxStringLength);
        }

        /// <summary>
        /// Remove a dynamic property from a attributetype.
        /// </summary>
        [HttpPost("RemoveProperty")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void RemoveProperty(string propertyName)
        {
            _objectService.RemoveProperty<EnovaAttributeType>(propertyName);
        }
    }
}
