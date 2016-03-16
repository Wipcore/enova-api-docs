using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Interfaces;
using System.Web.Http;
using Wipcore.eNova.Api.WebApi.Services;
using Wipcore.Enova.Api.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Route("api/{market}/[controller]")]
    public class ProductsController : ApiController
    {
        private readonly IObjectService _objectService;
        private readonly IProductService _productService;

        public ProductsController( IObjectService objectService, IProductService productService)
        {
            _objectService = objectService;
            _productService = productService;
        }

        [HttpGet(/*"{location}"*/)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] GetParametersModel getParameters)
        {
            return _objectService.Get<EnovaBaseProduct>(requestContext, getParameters);
        }

        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get(ContextModel requestContext, GetParametersModel getParameters, string identifier)
        {
            return _objectService.Get<EnovaBaseProduct>(requestContext, getParameters, identifier);
        }

        [HttpGet("{identifier}/variants")]
        public IEnumerable<IDictionary<string, object>> GetVariants(ContextModel requestContext, GetParametersModel getParameters, string identifier)
        {
            var members = _productService.GetVariants(identifier);
            return members == null ? null : _objectService.Get<EnovaBaseProduct>(requestContext, getParameters, members);
        }


        [HttpPut()]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _objectService.Save<EnovaBaseProduct>(requestContext, values);
        }
    }
}
