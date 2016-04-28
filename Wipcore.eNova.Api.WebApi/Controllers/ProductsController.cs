﻿using System;
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
    public class ProductsController : EnovaApiController
    {
        private readonly IObjectService _objectService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;
        private readonly IAttributeService _attributeService;

        public ProductsController( IObjectService objectService, IProductService productService, IWarehouseService warehouseService, IAttributeService attributeService)
        {
            _objectService = objectService;
            _productService = productService;
            _warehouseService = warehouseService;
            _attributeService = attributeService;
        }

        [HttpGet()]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] GetParametersModel getParameters)
        {
            return _objectService.Get<EnovaBaseProduct>(requestContext, getParameters);
        }

        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get(ContextModel requestContext, GetParametersModel getParameters, string identifier)
        {
            return _objectService.Get<EnovaBaseProduct>(requestContext, getParameters, identifier);
        }

        /// <summary>
        /// Get all variants of the product specified by identifier.
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="getParameters"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        [HttpGet("{identifier}/variants")]
        public IEnumerable<IDictionary<string, object>> GetVariants(ContextModel requestContext, GetParametersModel getParameters, string identifier)
        {
            var members = _productService.GetVariants(identifier);
            return members == null ? null : _objectService.Get<EnovaBaseProduct>(requestContext, getParameters, members);
        }

        /// <summary>
        /// Get all stock information for the product specified by identifier.
        /// </summary>
        /// <param name="requestContext">Context stuff</param>
        /// <param name="getParameters"></param>
        /// <param name="identifier"></param>
        /// <param name="warehouse"></param>
        /// <returns></returns>
        [HttpGet("{identifier}/stock")]
        public IEnumerable<IDictionary<string, object>> GetStock(ContextModel requestContext, GetParametersModel getParameters, string identifier, string warehouse = null)
        {
            var compartments = _warehouseService.GetWarehouseCompartments(identifier, warehouse);
            return compartments == null ? null : _objectService.Get<EnovaWarehouseCompartment>(requestContext, getParameters, compartments);
        }

        [HttpGet("{identifier}/attributes")]
        public IEnumerable<IDictionary<string, object>> GetAttributes(ContextModel requestContext, GetParametersModel getParameters, string identifier)
        {
            var attributes = _attributeService.GetAttributes<EnovaBaseProduct>(identifier);
            return attributes == null ? null : _objectService.Get<EnovaAttributeValue>(requestContext, getParameters, attributes);
        }
        
        [HttpPut()]
        public IDictionary<string, object> Put([FromUri]ContextModel requestContext, [FromBody] Dictionary<string, object> values)
        {
            return _objectService.Save<EnovaBaseProduct>(requestContext, values);
        }


    }
}
