using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Fasterflect;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.WebApi.Mappers;
using Wipcore.Enova.Api.WebApi.Models;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IConfigurationRoot _configuration;
        
        public ProductService(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        private Context Context
        {
            get { return EnovaContextProvider.GetCurrentContext(); }
        }

        public IEnumerable<IDictionary<string, object>> GetProducts(string properties, int page, int size, string sort)
        {
            int index = page > 0 ? (page - 1) * size : 0;
            
            if (String.IsNullOrEmpty(properties))
            {
                // Get properties from configuration
                properties = _configuration["properties:product"];
            }

            var products = EnovaBaseProduct.GetAll(Context);

            if (!String.IsNullOrEmpty(sort))
            {
                sort = sort.Replace(',', ';');
                products.Sort(sort);
            }

            products = products.GetRange(index, size);

            foreach (EnovaBaseProduct prod in products)
            {
                var dynamicObject = new Dictionary<string, Object>();
                foreach (var property in properties.Split(','))
                {
                    dynamicObject.Add(property, prod.GetProperty(property));
                }

                yield return dynamicObject;
            }
        }

        public IDictionary<string, object> GetProduct(string identifier, string properties)
        {
            var prod = this.Context.FindObject(identifier, typeof(EnovaBaseProduct));
            var dynamicObject = new Dictionary<string, Object>();
            
            if (String.IsNullOrEmpty(properties))
            {
                // Get properties from configuration
                properties = _configuration["properties:product"];
            }

            foreach (var property in properties.Split(','))
            {
                dynamicObject.Add(property, prod.GetProperty(property));
            }
            return dynamicObject;
        }

        public IEnumerable<IDictionary<string, object>> GetFilteredProducts(string properties, string filter, int page, int size)
        {
            int index = page > 0 ? (page - 1) * size : 0;

            if (String.IsNullOrEmpty(properties))
            {
                // Get properties from configuration
                properties = _configuration["properties:product"];
            }

            filter = filter.Replace(":", "=");
            var products = Context.Search(filter, typeof(EnovaBaseProduct));

            products = products.GetRange(index, size);

            foreach (EnovaBaseProduct prod in products)
            {
                var dynamicObject = new Dictionary<string, Object>();
                foreach (var property in properties.Split(','))
                {
                    dynamicObject.Add(property, prod.GetProperty(property));
                }

                yield return dynamicObject;
            }
        }
    }
}

