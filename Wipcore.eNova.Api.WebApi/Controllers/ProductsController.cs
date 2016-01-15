using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Wipcore.eNova.Api.WebApi.Helpers;
using Wipcore.eNova.Api.WebApi.Mappers;
using Wipcore.eNova.Api.WebApi.Models;
using Wipcore.eNova.Api.WebApi.Services;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wipcore.eNova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<IDictionary<string, object>> Get(string properties, int page, int size, string sort)
        {
            HttpContext.Response.Headers.Add("X-Paging-PageNo", page.ToString());
            HttpContext.Response.Headers.Add("X-Paging-PageSize", size.ToString());
            HttpContext.Response.Headers.Add("X-Paging-PageCount", "");
            HttpContext.Response.Headers.Add("X-Paging-TotalRecordCount", "");
            string prevPage = UrlHelper.GetRequestUrl(HttpContext).Replace("page=" + page.ToString(), "page=" + (page - 1).ToString());
            HttpContext.Response.Headers.Add("X-Paging-PreviousPage", prevPage);
            string nextPage = UrlHelper.GetRequestUrl(HttpContext).Replace("page=" + page.ToString(), "page=" + (page + 1).ToString());
            HttpContext.Response.Headers.Add("X-Paging-NextPage", nextPage);

            return _productService.GetProducts(properties, page, size, sort);
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
