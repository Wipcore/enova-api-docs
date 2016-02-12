using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Interfaces;
using System.Web.Http;
using Wipcore.Enova.Api.WebApi.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Route("api/{market}/[controller]")]
    public class CartsController : ApiController
    {
        private readonly IObjectService _objectService;
        private readonly ICartService _cartService;

        public CartsController(IObjectService objectService, ICartService cartService)
        {
            _objectService = objectService;
            _cartService = cartService;
        }

        [HttpGet(/*"{location}"*/)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] GetParametersModel getParameters)
        {
            return _objectService.Get<EnovaCart>(requestContext, getParameters);
        }

        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get(ContextModel requestContext, GetParametersModel getParameters, string identifier)
        {
            return _objectService.Get<EnovaCart>(requestContext, getParameters, identifier);
        }

        [HttpPut()]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _objectService.Save<EnovaCart>(requestContext, values);
        }

        [HttpPost()]
        public ICartModel Post([FromUri] ContextModel requestContext, [FromBody]CartModel cart)
        {
            if (String.IsNullOrEmpty(cart.Customer))
                cart.Customer = requestContext.Customer;
            return _cartService.CalculateCart(cart);
        }
    }
}
