using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Controllers;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class PriceListsController : EnovaApiController
    {
        private readonly IObjectService _objectService;

        public PriceListsController(IObjectService objectService)
        {
            _objectService = objectService;
        }

        /// <summary>
        /// Get a list of pricelists.
        /// </summary>
        [HttpGet()]
        [Authorize(Roles = AuthService.AdminRole)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.Get<EnovaPriceList>(requestContext, query);
        }

        /// <summary>
        /// Get a pricelist specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public IDictionary<string, object> Get([FromUri]ContextModel requestContext, [FromUri]QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaPriceList>(requestContext, query, identifier);
        }
    }
}
