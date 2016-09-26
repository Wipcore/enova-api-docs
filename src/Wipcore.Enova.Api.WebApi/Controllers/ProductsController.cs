using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Interfaces;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.OAuth;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : EnovaApiController
    {
        private readonly IObjectService _objectService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;
        private readonly IAttributeService _attributeService;

        public ProductsController(IExceptionService exceptionService, ObjectService objectService, IProductService productService, IWarehouseService warehouseService, IAttributeService attributeService)
            :base(exceptionService)
        {
            _objectService = objectService;
            _productService = productService;
            _warehouseService = warehouseService;
            _attributeService = attributeService;
        }

        /// <summary>
        /// Get a list of products.
        /// </summary>
        [HttpGet()]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.Get<EnovaBaseProduct>(requestContext, query);
        }

        /// <summary>
        /// Get a product specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaBaseProduct>(requestContext, query, identifier);
        }

        /// <summary>
        /// Get a product specified by id. 
        /// </summary>
        [HttpGet("id-{id}")]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, int id)
        {
            return _objectService.Get<EnovaBaseProduct>(requestContext, query, id);
        }

        /// <summary>
        /// Get all variants of the product specified by identifier.
        /// </summary>
        [HttpGet("{identifier}/variants")]
        public IEnumerable<IDictionary<string, object>> GetVariants(ContextModel requestContext, QueryModel query, string identifier)
        {
            var members = _productService.GetVariants(identifier);
            return members == null ? null : _objectService.Get<EnovaBaseProduct>(requestContext, query, members);
        }

        /// <summary>
        /// Get stock information for the product specified by identifier.
        /// </summary>
        [HttpGet("{identifier}/stock")]
        public IEnumerable<IDictionary<string, object>> GetStock(ContextModel requestContext, QueryModel query, string identifier, string warehouse = null)
        {
            var compartments = _warehouseService.GetWarehouseCompartments(identifier, warehouse);
            return compartments == null ? null : _objectService.Get<EnovaWarehouseCompartment>(requestContext, query, compartments);
        }

        /// <summary>
        /// Get attributes for the product specified by identifier.
        /// </summary>
        [HttpGet("{identifier}/attributes")]
        public IEnumerable<IDictionary<string, object>> GetAttributes(ContextModel requestContext, QueryModel query, string identifier)
        {
            var attributes = _attributeService.GetAttributes<EnovaBaseProduct>(identifier);
            return attributes == null ? null : _objectService.Get<EnovaAttributeValue>(requestContext, query, attributes);
        }
        
        /// <summary>
        /// Create or update a product.
        /// </summary>
        [HttpPut()]
        [Authorize(Roles = AuthService.AdminRole)]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _objectService.Save<EnovaBaseProduct>(requestContext, values);
        }


    }
}
