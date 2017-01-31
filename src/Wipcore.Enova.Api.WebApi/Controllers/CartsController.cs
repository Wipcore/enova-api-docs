using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Interfaces.Cart;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

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

        /// <summary>
        /// Get a list of carts.
        /// </summary>
        [HttpGet()]
        [Authorize(Roles = AuthService.AdminRole)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.Get<EnovaCart>(requestContext, query);
        }

        /// <summary>
        /// Get a cart specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        [Authorize]
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
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, int id)
        {
            var cart = _objectService.Get<EnovaCart>(requestContext, query, id);
            if (!_authService.AuthorizeAccess(EnovaCart.Find(_contextService.GetContext(), id).Customer?.Identifier))
                throw new HttpException(HttpStatusCode.Unauthorized, "This cart belongs to another customer.");

            return cart;
        }

        /// <summary>
        /// Get a cart mapped to a model.
        /// </summary>
        [HttpGet("AsModel")]
        [Authorize]
        public ICalculatedCartModel GetCartAsModel(ContextModel requestContext, string identifier = null, int id = 0)
        {
            var model = _cartService.GetCart(identifier, id);
            var context = _contextService.GetContext();
            var cart = context.FindObject<EnovaCart>(identifier) ?? context.FindObject<EnovaCart>(id);

            if (!_authService.AuthorizeAccess(cart?.Customer?.Identifier))
                throw new HttpException(HttpStatusCode.Unauthorized, "This cart belongs to another customer.");

            return model;
        }

        /// <summary>
        /// Create or update a cart.
        /// </summary>
        [HttpPost()]
        public ICartModel Post([FromUri] ContextModel requestContext, [FromBody]CartModel cart)
        {
            return _cartService.CalculateCart(cart);
        }

        /// <summary>
        /// Create or update a cart.
        /// </summary>
        [HttpPut()]
        [Authorize(Roles = AuthService.AdminRole)]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _objectService.Save<EnovaCart>(requestContext, values);
        }

        /// <summary>
        /// Create an order from given cart information. Returns order id.
        /// </summary>
        [HttpPut("createorder")]
        [Authorize(Roles = AuthService.AdminRole)]
        public int PutOrder([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
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
