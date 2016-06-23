﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Mvc;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.WebApi.Controllers;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class SectionsController : EnovaApiController
    {
        private readonly IObjectService _objectService;
        private readonly ISectionService _sectionService;

        public SectionsController(IObjectService objectService, ISectionService sectionService)
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
    }
}
