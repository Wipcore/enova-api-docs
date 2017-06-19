using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TaxesController : EnovaApiController
    {
        private readonly IObjectService _objectService;

        public TaxesController(IExceptionService exceptionService, IObjectService objectService)
            : base(exceptionService)
        {
            _objectService = objectService;
        }

        [HttpHead("{identifier}")]
        public void Head([FromUri]string identifier)
        {
            var found = _objectService.Exists<EnovaTax>(identifier);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        [HttpHead("id-{id}")]
        public void Head([FromUri]int id)
        {
            var found = _objectService.Exists<EnovaTax>(id);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Get a list of taxes.
        /// </summary>
        [HttpGet()]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.GetMany<EnovaTax>(requestContext, query);
        }

        /// <summary>
        /// Get an EnovaTax specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaTax>(requestContext, query, identifier);
        }

        /// <summary>
        /// Get an EnovaTax type specified by id.
        /// </summary>
        [HttpGet("id-{id}")]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, int id)
        {
            return _objectService.Get<EnovaTax>(requestContext, query, id);
        }

        /// <summary>
        /// Get EnovaTax specified by ids. 
        /// </summary>
        [HttpGet("ids/{ids}")]
        public IEnumerable<IDictionary<string, object>> GetManyIds([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromUri]string ids)
        {
            var listIds = ids.Split(',').Select(x => Convert.ToInt32(x));
            return _objectService.GetMany<EnovaTax>(requestContext, query, listIds);
        }

        /// <summary>
        /// Get EnovaTax specified by identifiers. 
        /// </summary>
        [HttpGet("identifiers/{identifiers}")]
        public IEnumerable<IDictionary<string, object>> GetManyIdentifiers([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromUri]string identifiers)
        {
            var listIdentifiers = identifiers.Split(',').Select(x => x.Trim());
            return _objectService.GetMany<EnovaTax>(requestContext, query, listIdentifiers);
        }

        /// <summary>
        /// Delete a tax.
        /// </summary>
        [HttpDelete("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(int id)
        {
            var success = _objectService.Delete<EnovaTax>(id);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Delete a tax.
        /// </summary>
        [HttpDelete("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(string identifier)
        {
            var success = _objectService.Delete<EnovaTax>(identifier);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }
    }
}
