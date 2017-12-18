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
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Attribute;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AttributeValuesController : EnovaApiController
    {
        private readonly IObjectService _objectService;
        private readonly IAttributeService _attributeService;

        public AttributeValuesController(IExceptionService exceptionService, IObjectService objectService, IAttributeService attributeService)
            : base(exceptionService)
        {
            _objectService = objectService;
            _attributeService = attributeService;
        }

        /// <summary>
        /// Check if the attributetype exists.
        /// </summary>
        [HttpHead("{identifier}")]
        [AllowAnonymous]
        public void Head(string identifier)
        {
            var found = _objectService.Exists<EnovaAttributeValue>(identifier);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Check if the attributetype exists.
        /// </summary>
        [HttpHead("id-{id}")]
        [AllowAnonymous]
        public void Head(int id)
        {
            var found = _objectService.Exists<EnovaAttributeValue>(id);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Get a list of attribute values.
        /// </summary>
        [HttpGet()]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AttributeValueModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.GetMany<EnovaAttributeValue>(requestContext, query);
        }

        /// <summary>
        /// Get an attribute value specified by identifier.
        /// </summary>
        [HttpGet("{identifier}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AttributeValueModel), (int)HttpStatusCode.Accepted)]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaAttributeValue>(requestContext, query, identifier);
        }

        /// <summary>
        /// Get an attribute value specified by id.
        /// </summary>
        [HttpGet("id-{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AttributeValueModel), (int)HttpStatusCode.Accepted)]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, int id)
        {
            return _objectService.Get<EnovaAttributeValue>(requestContext, query, id);
        }

        /// <summary>
        /// Get EnovaAttributeValue specified by ids. 
        /// </summary>
        [HttpGet("ids")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AttributeValueModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> GetManyIds([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string ids)
        {
            var listIds = ids.Split(',').Select(x => Convert.ToInt32(x.Trim())).Distinct();
            return _objectService.GetMany<EnovaAttributeValue>(requestContext, query, listIds);
        }

        /// <summary>
        /// Get EnovaAttributeValue specified by identifiers. 
        /// </summary>
        [HttpGet("identifiers")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AttributeValueModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> GetManyIdentifiers([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string identifiers)
        {
            var listIdentifiers = identifiers.Split(',').Select(x => x.Trim()).Distinct();
            return _objectService.GetMany<EnovaAttributeValue>(requestContext, query, listIdentifiers);
        }

        /// <summary>
        /// Get objects with attribute value.
        /// </summary>
        [HttpGet("id-{id}/objectsWithThisAttribute")]
        [AllowAnonymous]
        public IEnumerable<IDictionary<string, object>> GetWithAttribute(ContextModel requestContext, int id)
        {
            var objects = _attributeService.GetObjectsWithAttributeValue(id);
            return objects;
        }


        /// <summary>
        /// Delete an attributevalue.
        /// </summary>
        [HttpDelete("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(int id)
        {
            var success = _objectService.Delete<EnovaAttributeValue>(id);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Delete a attributevalue.
        /// </summary>
        [HttpDelete("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(string identifier)
        {
            var success = _objectService.Delete<EnovaAttributeValue>(identifier);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Add a dynamic property to a attributevalue.
        /// </summary>
        [HttpPost("AddProperty")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void AddProperty(string propertyName, BaseObject.PropertyType propertyType, bool languageDependant = false, int maxStringLength = 255)
        {
            _objectService.AddProperty<EnovaAttributeValue>(propertyName, propertyType, languageDependant, maxStringLength);
        }

        /// <summary>
        /// Remove a dynamic property from a attributevalue.
        /// </summary>
        [HttpPost("RemoveProperty")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void RemoveProperty(string propertyName)
        {
            _objectService.RemoveProperty<EnovaAttributeValue>(propertyName);
        }
    }
}
