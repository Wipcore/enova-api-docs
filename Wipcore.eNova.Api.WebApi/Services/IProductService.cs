using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Models;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public interface IProductService
    {
        IEnumerable<IDictionary<string, object>> GetProducts(string properties, int page, int size, string sort);
        IDictionary<string, object> GetProduct(string identifier, string properties);
        IEnumerable<IDictionary<string, object>> GetFilteredProducts(string properties, string filter, int page, int size);
    }
}
