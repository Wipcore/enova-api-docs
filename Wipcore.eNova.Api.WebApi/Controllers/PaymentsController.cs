using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Mvc;
using Wipcore.eNova.Api.WebApi.Services;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Models.Models;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Models.Interfaces;
using ApiController = Wipcore.Enova.Api.WebApi.Controllers.ApiController;

namespace Wipcore.eNova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Route("api/{market}/[controller]")]
    public class PaymentsController : ApiController
    {
        private readonly IObjectService _objectService;
        private readonly IPaymentService _paymentService;

        public PaymentsController(IObjectService objectService, IPaymentService paymentService)
        {
            _objectService = objectService;
            _paymentService = paymentService;
        }


        [HttpGet()]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] GetParametersModel getParameters)
        {
            return _objectService.Get<EnovaPayment>(requestContext, getParameters);
        }

        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get([FromUri]ContextModel requestContext, [FromUri]GetParametersModel getParameters, string identifier)
        {
            return _objectService.Get<EnovaPayment>(requestContext, getParameters, identifier);
        }
       

        [HttpPost()]
        public IPaymentModel Post([FromUri] ContextModel requestContext, [FromBody]PaymentModel payment)
        {
            return _paymentService.SetPayment(payment);
        }
    }
}
