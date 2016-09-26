using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Controllers;

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

        /// <summary>
        /// Get a list of payments.
        /// </summary>
        [HttpGet()]
        [Authorize(Roles = AuthService.AdminRole)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.Get<EnovaPayment>(requestContext, query);
        }

        /// <summary>
        /// Get a payment specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        [Authorize(Policy = CustomerUrlIdentifierPolicy.Name)]
        public IDictionary<string, object> Get([FromUri]ContextModel requestContext, [FromUri]QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaPayment>(requestContext, query, identifier);
        }
       
        /// <summary>
        /// Add a payment to an order.
        /// </summary>
        [HttpPost()]
        public IPaymentModel Post([FromUri] ContextModel requestContext, [FromBody]PaymentModel payment)
        {
            return _paymentService.SavePayment(payment);
        }
    }
}
