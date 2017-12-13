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
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class PaymentsController : EnovaApiController
    {
        private readonly IObjectService _objectService;
        private readonly IPaymentService _paymentService;

        public PaymentsController(IExceptionService exceptionService, IObjectService objectService, IPaymentService paymentService)
            : base(exceptionService)
        {
            _objectService = objectService;
            _paymentService = paymentService;
        }

        [HttpHead("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Head([FromUri]string identifier)
        {
            var found = _objectService.Exists<EnovaPayment>(identifier);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        [HttpHead("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Head([FromUri]int id)
        {
            var found = _objectService.Exists<EnovaPayment>(id);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Get a list of payments.
        /// </summary>
        [HttpGet()]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(PaymentModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.GetMany<EnovaPayment>(requestContext, query);
        }

        /// <summary>
        /// Get a payment specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(PaymentModel), (int)HttpStatusCode.Accepted)]
        public IDictionary<string, object> Get([FromUri]ContextModel requestContext, [FromUri]QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaPayment>(requestContext, query, identifier);
        }

        /// <summary>
        /// Get a payment specified by id. 
        /// </summary>
        [HttpGet("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(PaymentModel), (int)HttpStatusCode.Accepted)]
        public IDictionary<string, object> Get([FromUri]ContextModel requestContext, [FromUri]QueryModel query, int id)
        {
            return _objectService.Get<EnovaPayment>(requestContext, query, id);
        }

        /// <summary>
        /// Get EnovaPayment specified by ids. 
        /// </summary>
        [HttpGet("ids")]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(PaymentModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> GetManyIds([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string ids)
        {
            var listIds = ids.Split(',').Select(x => Convert.ToInt32(x.Trim())).Distinct();
            return _objectService.GetMany<EnovaPayment>(requestContext, query, listIds);
        }

        /// <summary>
        /// Get EnovaPayment specified by identifiers. 
        /// </summary>
        [HttpGet("identifiers")]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(PaymentModel), (int)HttpStatusCode.Accepted)]
        public IEnumerable<IDictionary<string, object>> GetManyIdentifiers([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromQuery]string identifiers)
        {
            var listIdentifiers = identifiers.Split(',').Select(x => x.Trim()).Distinct();
            return _objectService.GetMany<EnovaPayment>(requestContext, query, listIdentifiers);
        }

        /// <summary>
        /// Add a payment to an order.
        /// </summary>
        [HttpPost()]
        [AllowAnonymous]
        public IPaymentModel Post([FromUri] ContextModel requestContext, [FromBody]PaymentModel payment)
        {
            return _paymentService.SavePayment(payment);
        }

        /// <summary>
        /// Create or update an payment.
        /// </summary>
        [HttpPut()]
        [Authorize(Roles = AuthService.AdminRole)]
        [ProducesResponseType(typeof(PaymentModel), (int)HttpStatusCode.Accepted)]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _objectService.Save<EnovaPayment>(requestContext, values);
        }

        /// <summary>
        /// Delete a payment.
        /// </summary>
        [HttpDelete("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(int id)
        {
            var success = _objectService.Delete<EnovaPayment>(id);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Delete a payment.
        /// </summary>
        [HttpDelete("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(string identifier)
        {
            var success = _objectService.Delete<EnovaPayment>(identifier);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }
    }
}
