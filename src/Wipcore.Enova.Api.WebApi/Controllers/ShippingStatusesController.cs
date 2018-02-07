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
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.ShippingStatus;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ShippingStatusesController : EnovaApiController
    {
        private readonly IObjectService _objectService;

        public ShippingStatusesController(EnovaApiControllerDependencies dependencies)
            : base(dependencies)
        {
            _objectService = dependencies.ObjectService;
        }

        [HttpHead("{identifier}")]
        [AllowAnonymous]
        public void Head([FromUri]string identifier)
        {
            var found = _objectService.Exists<EnovaShippingStatus>(identifier);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        [HttpHead("id-{id}")]
        [AllowAnonymous]
        public void Head([FromUri]int id)
        {
            var found = _objectService.Exists<EnovaShippingStatus>(id);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Get a list of shippingstatuses.
        /// </summary>
        [HttpGet()]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ShippingStatusModel>), (int)HttpStatusCode.OK)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.GetMany<EnovaShippingStatus>(requestContext, query);
        }

        /// <summary>
        /// Get a shippingstatus specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ShippingStatusModel), (int)HttpStatusCode.OK)]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaShippingStatus>(requestContext, query, identifier);
        }

        /// <summary>
        /// Get a shippingstatus specified by id. 
        /// </summary>
        [HttpGet("id-{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ShippingStatusModel), (int)HttpStatusCode.OK)]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, int id)
        {
            return _objectService.Get<EnovaShippingStatus>(requestContext, query, id);
        }

        /// <summary>
        /// Get EnovaShippingStatus specified by ids. 
        /// </summary>
        [HttpGet("ids")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ShippingStatusModel>), (int)HttpStatusCode.OK)]
        public IEnumerable<IDictionary<string, object>> GetManyIds([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string ids)
        {
            var listIds = ids.Split(',').Select(x => Convert.ToInt32(x.Trim())).Distinct();
            return _objectService.GetMany<EnovaShippingStatus>(requestContext, query, listIds);
        }

        /// <summary>
        /// Get EnovaShippingStatus specified by identifiers. 
        /// </summary>
        [HttpGet("identifiers")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ShippingStatusModel>), (int)HttpStatusCode.OK)]
        public IEnumerable<IDictionary<string, object>> GetManyIdentifiers([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string identifiers)
        {
            var listIdentifiers = identifiers.Split(',').Select(x => x.Trim()).Distinct();
            return _objectService.GetMany<EnovaShippingStatus>(requestContext, query, listIdentifiers);
        }

        /// <summary>
        /// Create or update a shippingstatus.
        /// </summary>
        [HttpPut()]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(ShippingStatusModel), (int)HttpStatusCode.OK)]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _objectService.Save<EnovaShippingStatus>(requestContext, values);
        }

        /// <summary>
        /// Delete a shippingstatus.
        /// </summary>
        [HttpDelete("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(int id)
        {
            var success = _objectService.Delete<EnovaShippingStatus>(id);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Delete a shippingstatus.
        /// </summary>
        [HttpDelete("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(string identifier)
        {
            var success = _objectService.Delete<EnovaShippingStatus>(identifier);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Add a dynamic property to a shippingstatus.
        /// </summary>
        [HttpPost("AddProperty")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void AddProperty(string propertyName, BaseObject.PropertyType propertyType, bool languageDependant = false, int maxStringLength = 255)
        {
            _objectService.AddProperty<EnovaShippingStatus>(propertyName, propertyType, languageDependant, maxStringLength);
        }

        /// <summary>
        /// Remove a dynamic property from a shippingstatus.
        /// </summary>
        [HttpPost("RemoveProperty")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void RemoveProperty(string propertyName)
        {
            _objectService.RemoveProperty<EnovaShippingStatus>(propertyName);
        }
    }
}
