using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Wipcore.eNova.Api.WebApi.Services;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Interfaces;

namespace Wipcore.eNova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly IObjectService _objectService;

        public OrdersController(IObjectService objectService)
        {            
            _objectService = objectService;
        }

        [HttpGet(/*"{location}"*/)]
        public IEnumerable<IDictionary<string, object>> Get(string location = "default", string properties = null, int page = 0, int size = 20, string sort = null, string filter = null)
        {
            return _objectService.Get<EnovaOrder>(location: location, properties: properties, filter: filter, pageNumber: page, pageSize: size, sort: sort);
        }

        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get(string identifier, string properties = null)
        {
            return _objectService.Get<EnovaOrder>(identifier, properties);
        }
    }
}
