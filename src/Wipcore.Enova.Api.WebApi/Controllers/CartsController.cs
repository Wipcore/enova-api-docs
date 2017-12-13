using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class CartsController : EnovaApiController
    {
        private readonly IObjectService _objectService;
        private readonly ICartService _cartService;
        private readonly IAuthService _authService;
        private readonly IContextService _contextService;

        public CartsController(IExceptionService exceptionService, IObjectService objectService, ICartService cartService, IAuthService authService, IContextService contextService)
            : base(exceptionService)
        {
            _objectService = objectService;
            _cartService = cartService;
            _authService = authService;
            _contextService = contextService;
        }

        [HttpHead("{identifier}")]
        [Authorize]
        public void Head([FromUri]string identifier)
        {
            var found = _objectService.Exists<EnovaCart>(identifier);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        [HttpHead("id-{id}")]
        [Authorize]
        public void Head([FromUri]int id)
        {
            var found = _objectService.Exists<EnovaCart>(id);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Get a list of carts.
        /// </summary>
        [HttpGet()]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(CartModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.GetMany<EnovaCart>(requestContext, query);
        }

        /// <summary>
        /// Get a cart specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        [Authorize]
        [ProducesResponseType(typeof(CartModel), (int)HttpStatusCode.Accepted)]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, string identifier)
        {
            var cart = _objectService.Get<EnovaCart>(requestContext, query, identifier);
            if (!_authService.AuthorizeAccess(EnovaCart.Find(_contextService.GetContext(), identifier).Customer?.Identifier))
                throw new HttpException(HttpStatusCode.Unauthorized, "This cart belongs to another customer.");

            return cart;
        }

        /// <summary>
        /// Get a cart specified by id. 
        /// </summary>
        [HttpGet("id-{id}")]
        [Authorize]
        [ProducesResponseType(typeof(CartModel), (int)HttpStatusCode.Accepted)]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, int id)
        {
            var cart = _objectService.Get<EnovaCart>(requestContext, query, id);
            if (!_authService.AuthorizeAccess(EnovaCart.Find(_contextService.GetContext(), id).Customer?.Identifier))
                throw new HttpException(HttpStatusCode.Unauthorized, "This cart belongs to another customer.");

            return cart;
        }

        /// <summary>
        /// Get EnovaCart specified by ids. 
        /// </summary>
        [HttpGet("ids")]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(CartModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> GetManyIds([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string ids)
        {
            var listIds = ids.Split(',').Select(x => Convert.ToInt32(x.Trim())).Distinct();
            return _objectService.GetMany<EnovaCart>(requestContext, query, listIds);
        }

        /// <summary>
        /// Get EnovaCart specified by identifiers. 
        /// </summary>
        [HttpGet("identifiers")]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(CartModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> GetManyIdentifiers([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string identifiers)
        {
            var listIdentifiers = identifiers.Split(',').Select(x => x.Trim()).Distinct();
            return _objectService.GetMany<EnovaCart>(requestContext, query, listIdentifiers);
        }

        /// <summary>
        /// Get carts belonging to a customer.
        /// </summary>
        [HttpGet("ofcustomer-{identifier}")]
        [Authorize(Policy = CustomerUrlIdentifierPolicy.Name)]
        [ProducesResponseType(typeof(CartModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> GetCustomersCartsByIdentifier([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromUri]string identifier)
        {
            var carts = _cartService.GetCartsByCustomer(identifier);
            return _objectService.GetMany<EnovaCart>(requestContext, query, carts);
        }

        /// <summary>
        /// Get carts belonging to a customer.
        /// </summary>
        [HttpGet("ofcustomerid-{id}")]
        [Authorize(Policy = CustomerUrlIdPolicy.Name)]
        [ProducesResponseType(typeof(CartModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> GetCustomersCartsById([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromUri]int id)
        {
            var carts = _cartService.GetCartsByCustomer(null, id);
            return _objectService.GetMany<EnovaCart>(requestContext, query, carts);
        }

        /// <summary>
        /// Create or update a cart. Set calculateOnly to true to not save the cart.
        /// </summary>
        [HttpPut()]
        [Authorize]
        [ProducesResponseType(typeof(CartModel), (int)HttpStatusCode.Accepted)]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values, bool calculateOnly = false)
        {
            if (!_authService.AuthorizeAccess<EnovaCart>(_contextService.GetContext(), values, x => x.Customer?.ID))
                throw new HttpException(HttpStatusCode.Unauthorized, "This cart belongs to another customer.");

            return calculateOnly ? _objectService.Calculate<EnovaCart>(requestContext, values) : _objectService.Save<EnovaCart>(requestContext, values);
        }

        /// <summary>
        /// Create an order from given cart information. Returns order id.
        /// </summary>
        [HttpPut("createorder")]
        [Authorize]
        [ProducesResponseType(typeof(CartModel), (int)HttpStatusCode.Accepted)]
        public int PutOrder([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            if (!_authService.AuthorizeAccess<EnovaCart>(_contextService.GetContext(), values, x => x.Customer?.ID))
                throw new HttpException(HttpStatusCode.Unauthorized, "This cart belongs to another customer.");

            return _cartService.CreateOrderFromCart(requestContext, values);
        }

        /// <summary>
        /// Deletes the cart.
        /// </summary>
        [HttpDelete("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(int id)
        {
            var success = _objectService.Delete<EnovaCart>(id);
            if(!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Deletes the cart.
        /// </summary>
        [HttpDelete("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(string identifier)
        {
            var success = _objectService.Delete<EnovaCart>(identifier);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }
    }
}
