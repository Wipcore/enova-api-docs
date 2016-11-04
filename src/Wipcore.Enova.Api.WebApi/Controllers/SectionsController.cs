using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Controllers;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class SectionsController : EnovaApiController
    {
        private readonly IObjectService _objectService;
        private readonly ISectionService _sectionService;

        public SectionsController(IExceptionService exceptionService, IObjectService objectService, ISectionService sectionService)
            : base(exceptionService)
        {
            _objectService = objectService;
            _sectionService = sectionService;
        }

        /// <summary>
        /// Get a list of sections.
        /// </summary>
        [HttpGet()]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.Get<EnovaBaseProductSection>(requestContext, query);
        }

        /// <summary>
        /// Get a section specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaBaseProductSection>(requestContext, query, identifier);
        }

        /// <summary>
        /// Get a section specified by id. 
        /// </summary>
        [HttpGet("id-{id}")]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, int id)
        {
            return _objectService.Get<EnovaBaseProductSection>(requestContext, query, id);
        }

        /// <summary>
        /// Get children sections for section specified by identifier.
        /// </summary>
        [HttpGet("{identifier}/children")]
        public IEnumerable<IDictionary<string, object>> GetSubSections(ContextModel requestContext, QueryModel query, string identifier)
        {
            var children = _sectionService.GetSubSections(identifier);
            return _objectService.Get<EnovaBaseProductSection>(requestContext, query, children);
        }


        /// <summary>
        /// Get connected products for section specified by identifier.
        /// </summary>
        [HttpGet("{identifier}/products")]
        public IEnumerable<IDictionary<string, object>> GetProducts(ContextModel requestContext, QueryModel query, string identifier)
        {
            var items = _sectionService.GetProducts(identifier);
            return _objectService.Get<EnovaBaseProduct>(requestContext, query, items);
        }

        /// <summary>
        /// Create or update a section.
        /// </summary>
        [HttpPut()]
        [Authorize(Roles = AuthService.AdminRole)]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _objectService.Save<EnovaBaseProductSection>(requestContext, values);
        }

        /// <summary>
        /// Delete a section.
        /// </summary>
        [HttpDelete("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(int id)
        {
            var success = _objectService.Delete<EnovaBaseProductSection>(id);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Delete a section.
        /// </summary>
        [HttpDelete("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(string identifier)
        {
            var success = _objectService.Delete<EnovaBaseProductSection>(identifier);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }
    }
}
