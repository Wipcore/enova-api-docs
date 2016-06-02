using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Wipcore.eNova.Api.WebApi.Services;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Controllers;

namespace Wipcore.eNova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Route("api/{market}/[controller]")]
    public class PaymentsController : EnovaApiController
    {
        private readonly IObjectService _objectService;
        private readonly IPaymentService _paymentService;

        public PaymentsController(IObjectService objectService, IPaymentService paymentService)
        {
            _objectService = objectService;
            _paymentService = paymentService;
        }
        
        [HttpGet()]
        [Authorize(Roles = AuthService.AdminRole)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] GetParametersModel getParameters)
        {
            return _objectService.Get<EnovaPayment>(requestContext, getParameters);
        }

        [HttpGet("{identifier}")]
        [Authorize(Policy = CustomerUrlIdentifierPolicy.Name)]
        public IDictionary<string, object> Get([FromUri]ContextModel requestContext, [FromUri]GetParametersModel getParameters, string identifier)
        {
            return _objectService.Get<EnovaPayment>(requestContext, getParameters, identifier);
        }
       
        [HttpPost()]
        public IPaymentModel Post([FromUri] ContextModel requestContext, [FromBody]PaymentModel payment)
        {
            return _paymentService.SavePayment(payment);
        }
    }
}
