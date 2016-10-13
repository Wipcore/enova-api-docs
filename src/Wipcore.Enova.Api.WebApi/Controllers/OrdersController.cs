using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models.Cart;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Api.Models.Interfaces.Cart;
using Wipcore.Enova.Api.OAuth;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : EnovaApiController
    {
        private readonly IObjectService _objectService;
        private readonly IOrderService _orderService;
        private readonly IAuthService _authService;
        private readonly IContextService _contextService;

        public OrdersController(IExceptionService exceptionService, IObjectService objectService, IOrderService orderService, IAuthService authService, IContextService contextService)
            : base(exceptionService)
        {
            _objectService = objectService;
            _orderService = orderService;
            _authService = authService;
            _contextService = contextService;
        }

        /// <summary>
        /// Get a list of orders.
        /// </summary>
        [HttpGet()]
        [Authorize(Roles = AuthService.AdminRole)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.Get<EnovaOrder>(requestContext, query);
        }

        /// <summary>
        /// Get an order specified by identifier.
        /// </summary>
        [HttpGet("{identifier}")]
        [Authorize]
        public IDictionary<string, object> Get([FromUri]ContextModel requestContext, [FromUri]QueryModel query, string identifier)
        {
            var order = _objectService.Get<EnovaOrder>(requestContext, query, identifier);

            if(!_authService.AuthorizeAccess(EnovaOrder.Find(_contextService.GetContext(), identifier).Customer?.Identifier))
                throw new HttpException(HttpStatusCode.Unauthorized, "This order belongs to another customer.");

            return order;
        }

        /// <summary>
        /// Get an order specified by id.
        /// </summary>
        [HttpGet("id-{id}")]
        [Authorize]
        public IDictionary<string, object> Get([FromUri]ContextModel requestContext, [FromUri]QueryModel query, int id)
        {
            var order = _objectService.Get<EnovaOrder>(requestContext, query, id);

            if (!_authService.AuthorizeAccess(EnovaOrder.Find(_contextService.GetContext(), id).Customer?.Identifier))
                throw new HttpException(HttpStatusCode.Unauthorized, "This order belongs to another customer.");

            return order;
        }

        /// <summary>
        /// Get valid new shipping statuses for an order.
        /// </summary>
        [HttpGet("{identifier}/ValidStatusChanges")]
        [HttpGet("id-{id}/ValidStatusChanges")]
        [Authorize]
        public IDictionary<string, string> ValidShippingMoves([FromUri]ContextModel requestContext, string identifier = null, int id = 0, bool includeCurrentStatus = false)
        {
            var order = String.IsNullOrEmpty(identifier) ? EnovaOrder.Find(_contextService.GetContext(), id) : EnovaOrder.Find(_contextService.GetContext(), identifier);

            if (!_authService.AuthorizeAccess(order.Customer?.Identifier))
                throw new HttpException(HttpStatusCode.Unauthorized, "This order belongs to another customer.");

            return _orderService.GetValidShippingStatuses(order, includeCurrentStatus);
        }

        /// <summary>
        /// Create or update an order. Already existing orders cannot have basic order rows changed (quantity, types); instead create a new order.
        /// </summary>
        [HttpPost()]
        public ICartModel Post([FromUri] ContextModel requestContext, [FromBody]CartModel cart)
        {
            return _orderService.SaveOrder(cart);
        }

        /// <summary>
        /// Create or update an order.
        /// </summary>
        [HttpPut()]
        [Authorize(Roles = AuthService.AdminRole)]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _objectService.Save<EnovaOrder>(requestContext, values);
        }
    }
}
