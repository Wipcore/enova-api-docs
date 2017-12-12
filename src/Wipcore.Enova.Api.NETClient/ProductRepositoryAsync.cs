using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product;

namespace Wipcore.Enova.Api.NetClient
{
    public class ProductRepositoryAsync<TProductModel> where TProductModel : ProductModel
    {
        private readonly IApiRepositoryAsync _apiRepository;

        public ProductRepositoryAsync(IApiRepositoryAsync apiRepository)
        {
            _apiRepository = apiRepository;
        }

        public async Task<TProductModel> GetProduct(int id, QueryModel queryModel = null, ContextModel contextModel = null) =>
            await _apiRepository.GetObjectAsync<TProductModel>(id, queryModel, contextModel);

        public async Task<TProductModel> GetProduct(string identifier, QueryModel queryModel = null, ContextModel contextModel = null) =>
            await _apiRepository.GetObjectAsync<TProductModel>(identifier, queryModel, contextModel);

        public async Task<List<TProductModel>> GetProducts(IEnumerable<int> ids, QueryModel queryModel = null, ContextModel contextModel = null, ApiResponseHeadersModel headers = null)
        {
            var idsString = String.Join(",", ids);
            const string action = "ids";
            var extraParameters = new Dictionary<string, object>() { { "ids", idsString } };
            return (await _apiRepository.GetManyAsync<TProductModel>(queryModel, headers, contextModel, action, extraParameters: extraParameters)).ToList();
        }

        public async Task<List<TProductModel>> GetProducts(IEnumerable<string> identifiers, QueryModel queryModel = null, ContextModel contextModel = null, ApiResponseHeadersModel headers = null)
        {
            var idsString = String.Join(",", identifiers);
            const string action = "identifiers";
            var extraParameters = new Dictionary<string, object>() { { "identifiers", idsString } };
            return (await _apiRepository.GetManyAsync<TProductModel>(queryModel, headers, contextModel, action, extraParameters: extraParameters)).ToList();
        }

        public async Task<List<TProductModel>> GetProducts(QueryModel queryModel = null, ContextModel contextModel = null, ApiResponseHeadersModel headers = null) =>
            (await _apiRepository.GetManyAsync<TProductModel>(queryModel, headers, contextModel)).ToList();


        public async Task<List<TProductModel>> GetNextProductPage(ApiResponseHeadersModel previousRequestHeaders)
            => (await _apiRepository.GetNextPageAsync<TProductModel>(previousRequestHeaders)).ToList();

        public async Task<List<TProductModel>> GetPreviousProductPage(ApiResponseHeadersModel previousRequestHeaders)
            => (await _apiRepository.GetPreviousPageAsync<TProductModel>(previousRequestHeaders)).ToList();


        public async Task<TProductModel> CreateProduct(TProductModel product, ContextModel contextModel = null)
        {
            return (TProductModel) await _apiRepository.SaveObjectAsync<TProductModel>(JObject.FromObject(product), contextModel: contextModel, verifyIdentifierNotTaken: false);
        }

        public async Task<TProductModel> UpdateProduct(TProductModel product, ContextModel contextModel = null)
        {
            return (TProductModel) await _apiRepository.SaveObjectAsync<TProductModel>(JObject.FromObject(product), contextModel: contextModel, verifyIdentifierNotTaken: true);
        }

        public async Task<bool> DeleteProduct(string productIdentifier) => await _apiRepository.DeleteObjectAsync<TProductModel>(productIdentifier);

        public async Task<bool> DeleteProduct(int productId) => await _apiRepository.DeleteObjectAsync<TProductModel>(productId);
    }
}
