using System;
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
    public class CountriesController : EnovaApiController
    {
        private readonly IObjectService _objectService;

        public CountriesController(IObjectService objectService)
        {
            _objectService = objectService;
        }

        /// <summary>
        /// Get a list of countries.
        /// </summary>
        [HttpGet()]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.Get<EnovaCountry>(requestContext, query);
        }

        /// <summary>
        /// Get a country specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaCountry>(requestContext, query, identifier);
        }
    }
}
