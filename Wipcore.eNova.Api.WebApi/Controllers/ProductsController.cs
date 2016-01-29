using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Wipcore.eNova.Api.WebApi.Services;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Interfaces;
using System.Web.Http;
using Wipcore.eNova.Api.WebApi.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wipcore.eNova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Route("api/{market}/[controller]")]
    public class ProductsController : ApiController
    {
        private readonly IObjectService _objectService;

        public ProductsController( IObjectService objectService)
        {
            _objectService = objectService;
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

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        //[HttpGet]
        //[Route("[action]")]
        //public IEnumerable<IDictionary<string, object>> GetFilteredProducts(string properties, string filter, int page, int size)
        //{
        //    return _productService.GetFilteredProducts(properties, filter, page, size);
        //}
    }
}
