using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Mvc;
using Wipcore.eNova.Api.WebApi.Models;
using Wipcore.eNova.Api.WebApi.Services;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Interfaces;

namespace Wipcore.eNova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private readonly IObjectService _objectService;

        public CustomersController(IObjectService objectService)
        {            
            _objectService = objectService;
        }

        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] GetParametersModel getParameters)
        {
            return _objectService.Get<EnovaCustomer>(requestContext, getParameters);
        }

        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get(string identifier, string properties = null)
        {
            return _objectService.Get<EnovaCustomer>(identifier, properties);
        }
    }
}
