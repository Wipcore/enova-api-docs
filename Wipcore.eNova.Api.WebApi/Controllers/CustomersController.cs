using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityModel;
using IdentityServer4.Core.Resources;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.OAuth;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Route("api/{market}/[controller]")]
    public class CustomersController : EnovaApiController
    {
        private readonly IObjectService _objectService;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly ICustomerService _customerService;

        public CustomersController(IObjectService objectService, IOrderService orderService, ICartService cartService, ICustomerService customerService)
        {
            _objectService = objectService;
            _orderService = orderService;
            _cartService = cartService;
            _customerService = customerService;
        }

        [HttpGet()]
        [Authorize(Roles = "admin")]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] GetParametersModel getParameters)
        {
            return _objectService.Get<EnovaCustomer>(requestContext, getParameters);
        }

        [HttpGet("{identifier}")]
        [Authorize()]
        public /*IDictionary<string, object>*/ ActionResult Get([FromUri]ContextModel requestContext, [FromUri] GetParametersModel getParameters, string identifier)
        {
            if (!ValidateAuthorization(identifier))
                return HttpUnauthorized();

            return Ok(_objectService.Get<EnovaCustomer>(requestContext, getParameters, identifier));
        }

        [HttpGet("{identifier}/orders")]
        public IEnumerable<IDictionary<string, object>> GetOrders([FromUri]ContextModel requestContext, [FromUri] GetParametersModel getParameters, string identifier, string shippingStatus = null)
        {
            if (!ValidateAuthorization(identifier))
                return null;

            var orders = _orderService.GetOrdersByCustomer(identifier, shippingStatus);
            return _objectService.Get<EnovaOrder>(requestContext, getParameters, orders);
        }

        [HttpGet("{identifier}/carts")]
        public IEnumerable<IDictionary<string, object>> GetCarts([FromUri]ContextModel requestContext, [FromUri] GetParametersModel getParameters, string identifier)
        {
            if (!ValidateAuthorization(identifier))
                return null;

            var carts = _cartService.GetCartsByCustomer(identifier);
            return _objectService.Get<EnovaCart>(requestContext, getParameters, carts);
        }

        [HttpGet("{identifier}/addresses")]
        public IEnumerable<IDictionary<string, object>> GetAddresses([FromUri]ContextModel requestContext, [FromUri] GetParametersModel getParameters, string identifier, EnovaCustomerAddress.AddressTypeEnum? addressType = null)
        {
            if (!ValidateAuthorization(identifier))
                return null;

            var addresses = _customerService.GetAddresses(identifier, addressType);
            return _objectService.Get<EnovaCustomerAddress>(requestContext, getParameters, addresses);
        }

        [HttpPut()]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            //TODO can only update existing user if validated as it
            return _objectService.Save<EnovaCustomer>(requestContext, values);
        }

        private bool ValidateAuthorization(string identifier)
        {
            var username = User.FindFirst(AuthConstants.IdentifierClaim).Value;
            var role = User.FindFirst(JwtClaimTypes.Role).Value;

            //if the customer is not asking for its own information, and its not an admin asking, then deny
            if (username == identifier || role == AuthConstants.AdminRole)
                return true;

            //Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return false;
        }

    }
}
