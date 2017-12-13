using System;
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
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class CompaniesController : EnovaApiController
    {
        private readonly IObjectService _objectService;

        public CompaniesController(IExceptionService exceptionService, IObjectService objectService)
            : base(exceptionService)
        {
            _objectService = objectService;
        }

        [HttpHead("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Head([FromUri]string identifier)
        {
            var found = _objectService.Exists<EnovaCompany>(identifier);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        [HttpHead("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Head([FromUri]int id)
        {
            var found = _objectService.Exists<EnovaCompany>(id);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Get a list of companies.
        /// </summary>
        [HttpGet()]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(CompanyModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.GetMany<EnovaCompany>(requestContext, query);
        }

        /// <summary>
        /// Get a company specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(CompanyModel), (int)HttpStatusCode.Accepted)]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaCompany>(requestContext, query, identifier);
        }

        /// <summary>
        /// Get a company specified by id. 
        /// </summary>
        [HttpGet("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(CompanyModel), (int)HttpStatusCode.Accepted)]
        public IDictionary<string, object> Get([FromUri]ContextModel requestContext, [FromUri]QueryModel query, int id)
        {
            return _objectService.Get<EnovaCompany>(requestContext, query, id);
        }

        /// <summary>
        /// Get EnovaCompany specified by ids. 
        /// </summary>
        [HttpGet("ids")]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(CompanyModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> GetManyIds([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string ids)
        {
            var listIds = ids.Split(',').Select(x => Convert.ToInt32(x.Trim())).Distinct();
            return _objectService.GetMany<EnovaCompany>(requestContext, query, listIds);
        }

        /// <summary>
        /// Get EnovaCompany specified by identifiers. 
        /// </summary>
        [HttpGet("identifiers")]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(CompanyModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> GetManyIdentifiers([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string identifiers)
        {
            var listIdentifiers = identifiers.Split(',').Select(x => x.Trim()).Distinct();
            return _objectService.GetMany<EnovaCompany>(requestContext, query, listIdentifiers);
        }

        /// <summary>
        /// Create or update a company.
        /// </summary>
        [HttpPut()]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(CompanyModel), (int)HttpStatusCode.Accepted)]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _objectService.Save<EnovaCompany>(requestContext, values);
        }

        /// <summary>
        /// Delete a company.
        /// </summary>
        [HttpDelete("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(int id)
        {
            var success = _objectService.Delete<EnovaCompany>(id);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Delete a company.
        /// </summary>
        [HttpDelete("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(string identifier)
        {
            var success = _objectService.Delete<EnovaCompany>(identifier);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }
    }
}
