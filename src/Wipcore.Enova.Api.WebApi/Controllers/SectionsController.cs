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

        [HttpHead("{identifier}")]
        [AllowAnonymous]
        public void Head([FromUri]string identifier)
        {
            var found = _objectService.Exists<EnovaBaseProductSection>(identifier);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        [HttpHead("id-{id}")]
        [AllowAnonymous]
        public void Head([FromUri]int id)
        {
            var found = _objectService.Exists<EnovaBaseProductSection>(id);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Get a list of sections.
        /// </summary>
        [HttpGet()]
        [AllowAnonymous]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.GetMany<EnovaBaseProductSection>(requestContext, query);
        }

        /// <summary>
        /// Get a section specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        [AllowAnonymous]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaBaseProductSection>(requestContext, query, identifier);
        }

        /// <summary>
        /// Get a section specified by id. 
        /// </summary>
        [HttpGet("id-{id}")]
        [AllowAnonymous]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, int id)
        {
            return _objectService.Get<EnovaBaseProductSection>(requestContext, query, id);
        }

        /// <summary>
        /// Get EnovaBaseProductSection specified by ids. 
        /// </summary>
        [HttpGet("ids")]
        [AllowAnonymous]
        public IEnumerable<IDictionary<string, object>> GetManyIds([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string ids)
        {
            var listIds = ids.Split(',').Select(x => Convert.ToInt32(x.Trim())).Distinct();
            return _objectService.GetMany<EnovaBaseProductSection>(requestContext, query, listIds);
        }

        /// <summary>
        /// Get EnovaBaseProductSection specified by identifiers. 
        /// </summary>
        [HttpGet("identifiers")]
        [AllowAnonymous]
        public IEnumerable<IDictionary<string, object>> GetManyIdentifiers([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string identifiers)
        {
            var listIdentifiers = identifiers.Split(',').Select(x => x.Trim()).Distinct();
            return _objectService.GetMany<EnovaBaseProductSection>(requestContext, query, listIdentifiers);
        }

        /// <summary>
        /// Get children sections for section specified by identifier.
        /// </summary>
        [HttpGet("{identifier}/children")]
        [AllowAnonymous]
        public IEnumerable<IDictionary<string, object>> GetSubSections(ContextModel requestContext, QueryModel query, string identifier)
        {
            var children = _sectionService.GetSubSections(identifier);
            return _objectService.GetMany<EnovaBaseProductSection>(requestContext, query, children);
        }


        /// <summary>
        /// Get connected products for section specified by identifier.
        /// </summary>
        [HttpGet("{identifier}/products")]
        [AllowAnonymous]
        public IEnumerable<IDictionary<string, object>> GetProducts(ContextModel requestContext, QueryModel query, string identifier)
        {
            var items = _sectionService.GetProducts(identifier);
            return _objectService.GetMany<EnovaBaseProduct>(requestContext, query, items);
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

        /// <summary>
        /// Add a dynamic property to a section.
        /// </summary>
        [HttpPost("AddProperty")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void AddProperty(string propertyName, BaseObject.PropertyType propertyType, bool languageDependant = false, int maxStringLength = 255)
        {
            _objectService.AddProperty<EnovaBaseProductSection>(propertyName, propertyType, languageDependant, maxStringLength);
        }

        /// <summary>
        /// Remove a dynamic property from a section.
        /// </summary>
        [HttpPost("RemoveProperty")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void RemoveProperty(string propertyName)
        {
            _objectService.RemoveProperty<EnovaBaseProductSection>(propertyName);
        }
    }
}
