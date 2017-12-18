﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class WarehouseCompartmentsController : EnovaApiController
    {
        private readonly IObjectService _objectService;

        public WarehouseCompartmentsController(EnovaApiControllerDependencies dependencies)
            : base(dependencies)
        {
            _objectService = dependencies.ObjectService;
        }

        [HttpHead("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Head([FromUri]string identifier)
        {
            var found = _objectService.Exists<EnovaWarehouseCompartment>(identifier);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        [HttpHead("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Head([FromUri]int id)
        {
            var found = _objectService.Exists<EnovaWarehouseCompartment>(id);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Get a list of warehousescompartments.
        /// </summary>
        [HttpGet()]
        [Authorize(Roles = AuthService.AdminRole)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.GetMany<EnovaWarehouseCompartment>(requestContext, query);
        }

        /// <summary>
        /// Get a warehousecompartment specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaWarehouseCompartment>(requestContext, query, identifier);
        }

        /// <summary>
        /// Get an EnovaWarehouseCompartment type specified by id.
        /// </summary>
        [HttpGet("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, int id)
        {
            return _objectService.Get<EnovaWarehouseCompartment>(requestContext, query, id);
        }

        /// <summary>
        /// Get EnovaWarehouseCompartment specified by ids. 
        /// </summary>
        [HttpGet("ids")]
        [Authorize(Roles = AuthService.AdminRole)]
        public IEnumerable<IDictionary<string, object>> GetManyIds([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string ids)
        {
            var listIds = ids.Split(',').Select(x => Convert.ToInt32(x.Trim())).Distinct();
            return _objectService.GetMany<EnovaWarehouseCompartment>(requestContext, query, listIds);
        }

        /// <summary>
        /// Get EnovaWarehouseCompartment specified by identifiers. 
        /// </summary>
        [HttpGet("identifiers")]
        [Authorize(Roles = AuthService.AdminRole)]
        public IEnumerable<IDictionary<string, object>> GetManyIdentifiers([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string identifiers)
        {
            var listIdentifiers = identifiers.Split(',').Select(x => x.Trim()).Distinct();
            return _objectService.GetMany<EnovaWarehouseCompartment>(requestContext, query, listIdentifiers);
        }

        /// <summary>
        /// Delete a warehousecompartment.
        /// </summary>
        [HttpDelete("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(int id)
        {
            var success = _objectService.Delete<EnovaWarehouseCompartment>(id);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Delete a warehousecompartment.
        /// </summary>
        [HttpDelete("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(string identifier)
        {
            var success = _objectService.Delete<EnovaWarehouseCompartment>(identifier);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }
    }
}
