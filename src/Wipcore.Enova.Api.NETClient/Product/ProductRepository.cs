using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product;

namespace Wipcore.Enova.Api.NetClient.Product
{
    public class ProductRepository<TProductModel> where TProductModel : ProductModel
    {
        private readonly IApiRepository _apiRepository;

        public ProductRepository(IApiRepository apiRepository)
        {
            _apiRepository = apiRepository;
        }

        public TProductModel GetProduct(int id, QueryModel queryModel = null, ContextModel contextModel = null) =>
            _apiRepository.GetObject<TProductModel>(id, queryModel, contextModel);

        public TProductModel GetProduct(string identifier, QueryModel queryModel = null, ContextModel contextModel = null) =>
            _apiRepository.GetObject<TProductModel>(identifier, queryModel, contextModel);

        public List<TProductModel> GetProducts(IEnumerable<int> ids, QueryModel queryModel = null, ContextModel contextModel = null, ApiResponseHeadersModel headers = null)
        {
            var idsString = String.Join(",", ids);
            const string action = "ids";
            var extraParameters = new Dictionary<string, object>() { { "ids", idsString } };
            return _apiRepository.GetMany<TProductModel>(queryModel, headers, contextModel, action, extraParameters: extraParameters).ToList();
        }

        public List<TProductModel> GetProducts(IEnumerable<string> identifiers, QueryModel queryModel = null, ContextModel contextModel = null, ApiResponseHeadersModel headers = null)
        {
            var idsString = String.Join(",", identifiers);
            const string action = "identifiers";
            var extraParameters = new Dictionary<string, object>() { { "identifiers", idsString } };
            return _apiRepository.GetMany<TProductModel>(queryModel, headers, contextModel, action, extraParameters: extraParameters).ToList();
        }

        public List<TProductModel> GetProducts(QueryModel queryModel = null, ContextModel contextModel = null, ApiResponseHeadersModel headers = null) => 
            _apiRepository.GetMany<TProductModel>(queryModel, headers, contextModel).ToList();
        

        public List<TProductModel> GetNextProductPage(ApiResponseHeadersModel previousRequestHeaders)
            => _apiRepository.GetNextPage<TProductModel>(previousRequestHeaders).ToList();

        public List<TProductModel> GetPreviousProductPage(ApiResponseHeadersModel previousRequestHeaders)
            => _apiRepository.GetPreviousPage<TProductModel>(previousRequestHeaders).ToList();


        public TProductModel CreateProduct(TProductModel product, ContextModel contextModel = null)
        {
            return (TProductModel)_apiRepository.SaveObject<TProductModel>(JObject.FromObject(product), contextModel: contextModel, verifyIdentifierNotTaken: false);
        }

        public TProductModel UpdateProduct(TProductModel product, ContextModel contextModel = null)
        {
            return (TProductModel)_apiRepository.SaveObject<TProductModel>(JObject.FromObject(product), contextModel: contextModel, verifyIdentifierNotTaken: true);
        }

        public bool DeleteProduct(string productIdentifier) => _apiRepository.DeleteObject<TProductModel>(productIdentifier);

        public bool DeleteProduct(int productId) => _apiRepository.DeleteObject<TProductModel>(productId);
    }
}
