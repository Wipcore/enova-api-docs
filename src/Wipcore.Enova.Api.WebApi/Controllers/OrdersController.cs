using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : EnovaApiController
    {
        private readonly IObjectService _objectService;
        private readonly IOrderService _orderService;
        private readonly IAuthService _authService;
        private readonly IContextService _contextService;

        public OrdersController(EnovaApiControllerDependencies dependencies, IOrderService orderService, IAuthService authService, IContextService contextService)
            : base(dependencies)
        {
            _objectService = dependencies.ObjectService;
            _orderService = orderService;
            _authService = authService;
            _contextService = contextService;
        }

        [HttpHead("{identifier}")]
        [Authorize]
        public void Head([FromUri]string identifier)
        {
            var found = _objectService.Exists<EnovaOrder>(identifier);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        [HttpHead("id-{id}")]
        [Authorize]
        public void Head([FromUri]int id)
        {
            var found = _objectService.Exists<EnovaOrder>(id);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Get a list of orders.
        /// </summary>
        [HttpGet()]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(OrderModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.GetMany<EnovaOrder>(requestContext, query);
        }

        /// <summary>
        /// Get an order specified by identifier.
        /// </summary>
        [HttpGet("{identifier}")]
        [Authorize]
        [ProducesResponseType(typeof(OrderModel), (int)HttpStatusCode.Accepted)]
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
        [ProducesResponseType(typeof(OrderModel), (int)HttpStatusCode.Accepted)]
        public IDictionary<string, object> Get([FromUri]ContextModel requestContext, [FromUri]QueryModel query, int id)
        {
            var order = _objectService.Get<EnovaOrder>(requestContext, query, id);

            if (!_authService.AuthorizeAccess(EnovaOrder.Find(_contextService.GetContext(), id).Customer?.Identifier))
                throw new HttpException(HttpStatusCode.Unauthorized, "This order belongs to another customer.");

            return order;
        }

        /// <summary>
        /// Get EnovaOrder specified by ids. 
        /// </summary>
        [HttpGet("ids")]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(OrderModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> GetManyIds([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string ids)
        {
            var listIds = ids.Split(',').Select(x => Convert.ToInt32(x.Trim())).Distinct();
            return _objectService.GetMany<EnovaOrder>(requestContext, query, listIds);
        }

        /// <summary>
        /// Get EnovaOrder specified by identifiers. 
        /// </summary>
        [HttpGet("identifiers")]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(OrderModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> GetManyIdentifiers([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string identifiers)
        {
            var listIdentifiers = identifiers.Split(',').Select(x => x.Trim()).Distinct();
            return _objectService.GetMany<EnovaOrder>(requestContext, query, listIdentifiers);
        }

        /// <summary>
        /// Get orders belonging to a customer.
        /// </summary>
        [HttpGet("ofcustomer-{identifier}")]
        [Authorize(Policy = CustomerUrlIdentifierPolicy.Name)]
        [ProducesResponseType(typeof(OrderModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> GetCustomersCartsByIdentifier([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromUri]string identifier, [FromUri]string shippingStatus)
        {
            var orders = _orderService.GetOrdersByCustomer(0, identifier, shippingStatus);
            return _objectService.GetMany<EnovaOrder>(requestContext, query, orders);
        }

        /// <summary>
        /// Get orders belonging to a customer.
        /// </summary>
        [HttpGet("ofcustomerid-{id}")]
        [Authorize(Policy = CustomerUrlIdPolicy.Name)]
        [ProducesResponseType(typeof(OrderModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> GetCustomersCartsById([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromUri]int id, [FromUri]string shippingStatus)
        {
            var orders = _orderService.GetOrdersByCustomer(id, null, shippingStatus);
            return _objectService.GetMany<EnovaOrder>(requestContext, query, orders);
        }

        /// <summary>
        /// Get valid new shipping statuses for an order.
        /// </summary>
        [HttpGet("{identifier}/ValidStatusChanges")]
        [HttpGet("id-{id}/ValidStatusChanges")]
        [Authorize]
        public IDictionary<string, string> ValidShippingMoves([FromUri]ContextModel requestContext, string identifier = null, int id = 0, bool includeCurrentStatus = false, bool allValidIfNoStatus = true)
        {
            var order = String.IsNullOrEmpty(identifier) ? EnovaOrder.Find(_contextService.GetContext(), id) : EnovaOrder.Find(_contextService.GetContext(), identifier);

            if (!_authService.AuthorizeAccess(order.Customer?.Identifier))
                throw new HttpException(HttpStatusCode.Unauthorized, "This order belongs to another customer.");

            return _orderService.GetValidShippingStatuses(order, includeCurrentStatus, allValidIfNoStatus);
        }
        
        /// <summary>
        /// Create or update an order.
        /// </summary>
        [HttpPut()]
        [Authorize()]
        [ProducesResponseType(typeof(OrderModel), (int)HttpStatusCode.Accepted)]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            if (!_authService.AuthorizeAccess<EnovaOrder>(_contextService.GetContext(), values, x => x.Customer?.ID))
                throw new HttpException(HttpStatusCode.Unauthorized, "This order belongs to another customer.");

            return _objectService.Save<EnovaOrder>(requestContext, values);
        }

        /// <summary>
        /// Delete an order. Note: it is usually better to put it in scrap status.
        /// </summary>
        [HttpDelete("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(int id)
        {
            var success = _objectService.Delete<EnovaOrder>(id);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Delete an order. Note: it is usually better to put it in scrap status.
        /// </summary>
        [HttpDelete("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(string identifier)
        {
            var success = _objectService.Delete<EnovaOrder>(identifier);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Add a dynamic property to a order.
        /// </summary>
        [HttpPost("AddProperty")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void AddProperty(string propertyName, BaseObject.PropertyType propertyType, bool languageDependant = false, int maxStringLength = 255)
        {
            _objectService.AddProperty<EnovaOrder>(propertyName, propertyType, languageDependant, maxStringLength);
        }

        /// <summary>
        /// Remove a dynamic property from a order.
        /// </summary>
        [HttpPost("RemoveProperty")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void RemoveProperty(string propertyName)
        {
            _objectService.RemoveProperty<EnovaOrder>(propertyName);
        }
    }
}
