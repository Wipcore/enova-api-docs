using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Interfaces;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Route("api/{market}/[controller]")]
    public class CustomersController : ApiController
    {
        private readonly IObjectService _objectService;

        public CustomersController(IObjectService objectService)
        {            
            _objectService = objectService;
        }

        [HttpGet()]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] GetParametersModel getParameters)
        {
            return _objectService.Get<EnovaCustomer>(requestContext, getParameters);
        }

        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get([FromUri]ContextModel requestContext, [FromUri] GetParametersModel getParameters, string identifier)
        {
            return _objectService.Get<EnovaCustomer>(requestContext, getParameters, identifier);
        }

        [HttpPut()]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _objectService.Save<EnovaCustomer>(requestContext, values);
        }

    }
}
