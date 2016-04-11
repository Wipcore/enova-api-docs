using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Mvc;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Route("api/{market}/[controller]")]
    public class SectionsController : ApiController
    {
        private readonly IObjectService _objectService;
        private readonly IContextService _contextService;

        public SectionsController(IObjectService objectService, IContextService contextService)
        {
            _objectService = objectService;
            _contextService = contextService;
        }

        [HttpGet()]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] GetParametersModel getParameters)
        {
            return _objectService.Get<EnovaBaseProductSection>(requestContext, getParameters);
        }

        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get(ContextModel requestContext, GetParametersModel getParameters, string identifier)
        {
            return _objectService.Get<EnovaBaseProductSection>(requestContext, getParameters, identifier);
        }

        [HttpGet("{identifier}/products")]
        public IEnumerable<IDictionary<string, object>> GetProducts(ContextModel requestContext, GetParametersModel getParameters, string identifier)
        {
            var context = _contextService.GetContext();
            var section = EnovaBaseProductSection.Find(context, identifier);
            var items = section.GetItems(typeof (EnovaBaseProduct));
            return _objectService.Get<EnovaBaseProduct>(requestContext, getParameters, items);
        }
    }
}
