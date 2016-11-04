﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Helpers;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : EnovaApiController
    {
        private readonly IObjectService _objectService;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly ICustomerService _customerService;
        private readonly IAuthService _authService;

        public CustomersController(IExceptionService exceptionService, IObjectService objectService, IOrderService orderService, ICartService cartService, ICustomerService customerService, IAuthService authService)
            : base(exceptionService)
        {
            _objectService = objectService;
            _orderService = orderService;
            _cartService = cartService;
            _customerService = customerService;
            _authService = authService;
        }

        /// <summary>
        /// Get a list of customers.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = AuthService.AdminRole)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.Get<EnovaCustomer>(requestContext, query);
        }

        /// <summary>
        /// Get a customer specified by identifier.
        /// </summary>
        [HttpGet("{identifier}")]
        [Authorize(Policy = CustomerUrlIdentifierPolicy.Name)]
        public IDictionary<string, object> Get([FromUri]ContextModel requestContext, [FromUri] QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaCustomer>(requestContext, query, identifier);
        }

        /// <summary>
        /// Get a customer specified by id.
        /// </summary>
        [HttpGet("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public IDictionary<string, object> Get([FromUri]ContextModel requestContext, [FromUri] QueryModel query, int id)
        {
            return _objectService.Get<EnovaCustomer>(requestContext, query, id);
        }

        /// <summary>
        /// Get orders beloning to the customer specified by identifier.
        /// </summary>
        [HttpGet("{identifier}/orders")]
        [Authorize(Policy = CustomerUrlIdentifierPolicy.Name)]
        public IEnumerable<IDictionary<string, object>> GetOrders([FromUri]ContextModel requestContext, [FromUri] QueryModel query, string identifier, string shippingStatus = null)
        {
            var orders = _orderService.GetOrdersByCustomer(identifier, shippingStatus);
            return _objectService.Get<EnovaOrder>(requestContext, query, orders);
        }

        /// <summary>
        /// Get carts beloning to the customer specified by identifier.
        /// </summary>
        [HttpGet("{identifier}/carts")]
        [Authorize(Policy = CustomerUrlIdentifierPolicy.Name)]
        public IEnumerable<IDictionary<string, object>> GetCarts([FromUri]ContextModel requestContext, [FromUri] QueryModel query, string identifier)
        {
            var carts = _cartService.GetCartsByCustomer(identifier);
            return _objectService.Get<EnovaCart>(requestContext, query, carts);
        }

        /// <summary>
        /// Get addresses beloning to the customer specified by identifier.
        /// </summary>
        [HttpGet("{identifier}/addresses")]
        [Authorize(Policy = CustomerUrlIdentifierPolicy.Name)]
        public IEnumerable<IDictionary<string, object>> GetAddresses([FromUri]ContextModel requestContext, [FromUri] QueryModel query, string identifier, EnovaCustomerAddress.AddressTypeEnum? addressType = null)
        {
            var addresses = _customerService.GetAddresses(identifier, addressType);
            return _objectService.Get<EnovaCustomerAddress>(requestContext, query, addresses);
        }

        /// <summary>
        /// Create or update a customer.
        /// </summary>
        [HttpPut()]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _customerService.SaveCustomer(requestContext, values);
        }

        /// <summary>
        /// Delete a customer.
        /// </summary>
        [HttpDelete("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(int id)
        {
            var success = _objectService.Delete<EnovaCustomer>(id);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Delete a customer.
        /// </summary>
        [HttpDelete("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(string identifier)
        {
            var success = _objectService.Delete<EnovaCustomer>(identifier);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }
    }
}
