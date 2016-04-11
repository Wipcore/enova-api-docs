using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Mvc;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Route("api/{market}/[controller]")]
    public class ShippingTypeCostsController : ApiController
    {
        private readonly IObjectService _objectService;

        public ShippingTypeCostsController(IObjectService objectService)
        {
            _objectService = objectService;
        }

        [HttpGet()]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] GetParametersModel getParameters)
        {
            return _objectService.Get<EnovaShippingTypeCost>(requestContext, getParameters);
        }

        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get(ContextModel requestContext, GetParametersModel getParameters, string identifier)
        {
            return _objectService.Get<EnovaShippingTypeCost>(requestContext, getParameters, identifier);
        }
    }
}
