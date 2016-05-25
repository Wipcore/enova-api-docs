using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Interfaces;
using System.Web.Http;
using Microsoft.AspNet.Authorization;
using Wipcore.eNova.Api.WebApi.Helpers;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Api.OAuth;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Route("api/{market}/[controller]")]
    public class CartsController : EnovaApiController
    {
        private readonly IObjectService _objectService;
        private readonly ICartService _cartService;
        private readonly IAuthService _authService;
        private readonly IContextService _contextService;

        public CartsController(IObjectService objectService, ICartService cartService, IAuthService authService, IContextService contextService)
        {
            _objectService = objectService;
            _cartService = cartService;
            _authService = authService;
            _contextService = contextService;
        }

        [HttpGet()]
        [Authorize(Roles = AuthService.AdminRole)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] GetParametersModel getParameters)
        {
            return _objectService.Get<EnovaCart>(requestContext, getParameters);
        }

        [HttpGet("{identifier}")]
        [Authorize]
        public IDictionary<string, object> Get(ContextModel requestContext, GetParametersModel getParameters, string identifier)
        {
            var cart = _objectService.Get<EnovaCart>(requestContext, getParameters, identifier);
            if (!_authService.AuthorizeAccess(EnovaCart.Find(_contextService.GetContext(), identifier).Customer?.Identifier))
                throw new HttpException(HttpStatusCode.Unauthorized, "This cart belongs to another customer.");

            return cart;
        }
        
        [HttpPost()]
        public ICartModel Post([FromUri] ContextModel requestContext, [FromBody]CartModel cart)
        {
            return _cartService.CalculateCart(cart);
        }
    }
}
