﻿using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ShippingStatusesController : EnovaApiController
    {
        private readonly IObjectService _objectService;

        public ShippingStatusesController(IExceptionService exceptionService, IObjectService objectService)
            : base(exceptionService)
        {
            _objectService = objectService;
        }

        /// <summary>
        /// Get a list of shippingstatuses.
        /// </summary>
        [HttpGet()]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.Get<EnovaShippingStatus>(requestContext, query);
        }

        /// <summary>
        /// Get a shippingstatus specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaShippingStatus>(requestContext, query, identifier);
        }

        /// <summary>
        /// Get a shippingstatus specified by id. 
        /// </summary>
        [HttpGet("id-{id}")]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, int id)
        {
            return _objectService.Get<EnovaShippingStatus>(requestContext, query, id);
        }

        /// <summary>
        /// Create or update a shippingstatus.
        /// </summary>
        [HttpPut()]
        [Authorize(Roles = AuthService.AdminRole)]
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
    }
}
