using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Wipcore.eNova.Api.WebApi.Helpers;
using Wipcore.eNova.Api.WebApi.Mappers;
using Wipcore.eNova.Api.WebApi.Models;
using Wipcore.eNova.Api.WebApi.Services;
using Wipcore.eNova.Api.Interfaces;
using Wipcore.Enova.Core;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wipcore.eNova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IObjectService _objectService;

        public ProductsController(IProductService productService, IObjectService objectService)
        {
            _productService = productService;
            _objectService = objectService;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<IDictionary<string, object>> Get(string properties = null, int page = 0, int size = 20, string sort = null, string filter = null)
        {
            

            //return _productService.GetProducts(properties, page, size, sort);
            return _objectService.Get<EnovaBaseProduct>(properties: properties, filter: filter, pageNumber: page, pageSize: size, sort: sort);
        }
        
        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get(string identifier, string properties)
        {
            return _productService.GetProduct(identifier, properties);
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

        [HttpGet]
        [Route("[action]")]
        public IEnumerable<IDictionary<string, object>> GetFilteredProducts(string properties, string filter, int page, int size)
        {
            return _productService.GetFilteredProducts(properties, filter, page, size);
        }
    }
}
