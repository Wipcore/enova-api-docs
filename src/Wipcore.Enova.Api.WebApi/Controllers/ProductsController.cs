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
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Core;

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

        [HttpHead("{identifier}")]
        public void Head([FromUri]string identifier)
        {
            var found = _objectService.Exists<EnovaBaseProduct>(identifier);
            if (!found)
                Response.StatusCode = (int) HttpStatusCode.NotFound;
        }

        [HttpHead("id-{id}")]
        public void Head([FromUri]int id)
        {
            var found = _objectService.Exists<EnovaBaseProduct>(id);
            if (!found)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Get a list of products.
        /// </summary>
        [HttpGet()]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.GetMany<EnovaBaseProduct>(requestContext, query);
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
        /// Get products specified by ids. 
        /// </summary>
        [HttpGet("ids/{ids}")]
        public IEnumerable<IDictionary<string, object>> GetManyIds([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromUri]string ids)
        {
            var listIds = ids.Split(',').Select(x => Convert.ToInt32(x));
            return _objectService.GetMany<EnovaBaseProduct>(requestContext, query, listIds);
        }

        /// <summary>
        /// Get products specified by identifiers. 
        /// </summary>
        [HttpGet("identifiers/{identifiers}")]
        public IEnumerable<IDictionary<string, object>> GetManyIdentifiers([FromUri]ContextModel requestContext, [FromUri]QueryModel query, [FromUri]string identifiers)
        {
            var listIdentifiers = identifiers.Split(',').Select(x => x.Trim());
            return _objectService.GetMany<EnovaBaseProduct>(requestContext, query, listIdentifiers);
        }

        /// <summary>
        /// Get all variants of the product specified by identifier.
        /// </summary>
        [HttpGet("{identifier}/variants")]
        public IEnumerable<IDictionary<string, object>> GetVariants(ContextModel requestContext, QueryModel query, string identifier)
        {
            var members = _productService.GetVariants(identifier);
            return members == null ? null : _objectService.GetMany<EnovaBaseProduct>(requestContext, query, members);
        }

        /// <summary>
        /// Get stock information for the product specified by identifier.
        /// </summary>
        [HttpGet("{identifier}/stock")]
        public IEnumerable<IDictionary<string, object>> GetStock(ContextModel requestContext, QueryModel query, string identifier, string warehouse = null)
        {
            var compartments = _warehouseService.GetWarehouseCompartments(identifier, warehouse);
            return compartments == null ? null : _objectService.GetMany<EnovaWarehouseCompartment>(requestContext, query, compartments);
        }

        /// <summary>
        /// Get attributes for the product specified by identifier.
        /// </summary>
        [HttpGet("{identifier}/attributes")]
        public IEnumerable<IDictionary<string, object>> GetAttributes(ContextModel requestContext, QueryModel query, string identifier)
        {
            var attributes = _attributeService.GetAttributes<EnovaBaseProduct>(identifier);
            return attributes == null ? null : _objectService.GetMany<EnovaAttributeValue>(requestContext, query, attributes);
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

        /// <summary>
        /// Delete a product.
        /// </summary>
        [HttpDelete("id-{id}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(int id)
        {
            var success = _objectService.Delete<EnovaBaseProduct>(id);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

        /// <summary>
        /// Delete a product.
        /// </summary>
        [HttpDelete("{identifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public void Delete(string identifier)
        {
            var success = _objectService.Delete<EnovaBaseProduct>(identifier);
            if (!success)
                throw new HttpException(HttpStatusCode.NotFound, "The object does not exist.");
        }

    }
}
