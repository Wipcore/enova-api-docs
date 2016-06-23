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
    public class ShippingStatusesController : EnovaApiController
    {
        private readonly IObjectService _objectService;

        public ShippingStatusesController(IObjectService objectService)
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
    }
}
