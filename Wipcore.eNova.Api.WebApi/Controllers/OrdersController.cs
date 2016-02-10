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
    [Route("api/{market}/[controller]")]
    public class OrdersController : ApiController
    {
        private readonly IObjectService _objectService;
        private readonly IOrderService _orderService;

        public OrdersController(IObjectService objectService, IOrderService orderService)
        {
            _objectService = objectService;
            _orderService = orderService;
        }

        [HttpGet()]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] GetParametersModel getParameters)
        {
            return _objectService.Get<EnovaOrder>(requestContext, getParameters);
        }

        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get([FromUri]ContextModel requestContext, [FromUri]GetParametersModel getParameters, string identifier)
        {
            return _objectService.Get<EnovaOrder>(requestContext, getParameters, identifier);
        }

        [HttpPut()]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _objectService.Save<EnovaOrder>(requestContext, values);
        }

        [HttpPost()]
        public ICartModel Post([FromUri] ContextModel requestContext, [FromBody]CartModel cart)
        {
            if (String.IsNullOrEmpty(cart.Customer))
                cart.Customer = requestContext.Customer;
            return _orderService.CreateOrder(cart);
        }
    }
}
