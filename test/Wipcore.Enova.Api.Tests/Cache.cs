using System.Linq;
using Microsoft.AspNetCore.Http;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product;
using Wipcore.Enova.Api.NetClient.Customer;
using Wipcore.Enova.Api.NetClient.Product;
using Xunit;

namespace Wipcore.Enova.Api.Tests
{
    [Collection("WebApiCollection")]
    public class Cache : IClassFixture<TestService>
    {
        private readonly TestService _testService;
        private readonly ProductRepository<ProductModel> _productRepository;
        private readonly CustomerRepository<CustomerModel, CartModel, OrderModel> _customerRepository;

        public Cache(TestService testService)
        {
            _testService = testService;
            _productRepository = (ProductRepository<ProductModel>)_testService.Server.Host.Services.GetService(typeof(ProductRepository<ProductModel>));
            _customerRepository = (CustomerRepository<CustomerModel, CartModel, OrderModel>)_testService.Server.Host.Services.GetService(typeof(CustomerRepository<CustomerModel, CartModel, OrderModel>));
        }

        [Fact]
        public void CanGetResultFromCache()
        {
            _testService.SetupHttpContext(new DefaultHttpContext() { TraceIdentifier = WipConstants.InternalHttpContextIdentifier });

            //send first request
            var headers = new ApiResponseHeadersModel();
            var query = new QueryModel() { Size = 5, Properties = "ID" };
            var productIdSum = _productRepository.GetProducts(query).Sum(x => x.ID);

            //ask again, answer should be the same and it should be returned from cache
            var productIdSumCache = _productRepository.GetProducts(query, headers: headers).Sum(x => x.ID);

            Assert.Equal(productIdSum, productIdSumCache);
            Assert.True(headers.CacheStatus == CacheStatus.Hit);
        }

        [Fact]
        public void CanGetResultFromCacheDependingOnArguments()
        {
            _testService.SetupHttpContext(new DefaultHttpContext() { TraceIdentifier = WipConstants.InternalHttpContextIdentifier });

            var arguments1 = new QueryModel(){Sort = "Identifier", Size = 5};
            var arguments2 = new QueryModel() { Sort = "Identifier", Size = 10 };
            var arguments3 = new QueryModel() { Sort = "Name", Size = 5 };

            //3 different questions should get different responses and not be found in cache
            var headers = new ApiResponseHeadersModel();
            var productIdSum1 = _productRepository.GetProducts(arguments1, headers: headers).Sum(x => x.ID);
            Assert.True(headers.CacheStatus == CacheStatus.Miss);
            var productIdSum2 = _productRepository.GetProducts(arguments2, headers: headers).Sum(x => x.ID);
            Assert.True(headers.CacheStatus == CacheStatus.Miss);
            var productIdSum3 = _productRepository.GetProducts(arguments3, headers: headers).Sum(x => x.ID);
            Assert.True(headers.CacheStatus == CacheStatus.Miss);

            Assert.NotEqual(productIdSum1, productIdSum2);
            Assert.NotEqual(productIdSum1, productIdSum3);

            //ask again and all 3 should be in cache
            var productIdSumCache1 = _productRepository.GetProducts(arguments1, headers: headers).Sum(x => x.ID);
            Assert.True(headers.CacheStatus == CacheStatus.Hit);
            var productIdSumCache2 = _productRepository.GetProducts(arguments2, headers: headers).Sum(x => x.ID);
            Assert.True(headers.CacheStatus == CacheStatus.Hit);
            var productIdSumCache3 = _productRepository.GetProducts(arguments3, headers: headers).Sum(x => x.ID);
            Assert.True(headers.CacheStatus == CacheStatus.Hit);

            Assert.Equal(productIdSum1, productIdSumCache1);
            Assert.Equal(productIdSum2, productIdSumCache2);
            Assert.Equal(productIdSum3, productIdSumCache3);
        }


        [Fact]
        public void CanBypassCache()
        {
            _testService.SetupHttpContext(new DefaultHttpContext() { TraceIdentifier = WipConstants.InternalHttpContextIdentifier });

            var headers = new ApiResponseHeadersModel();
            _productRepository.GetProducts(headers:headers);
            var productsIdSumCache = _productRepository.GetProducts(headers: headers).Sum(x => x.ID);  //2nd time should be in cache
            Assert.True(headers.CacheStatus == CacheStatus.Hit);

            //ask with cache set to false. Should bypass cache but get the same answer
            var productsIdSumCacheBypass = _productRepository.GetProducts(new QueryModel() {Cache = false}, headers: headers).Sum(x => x.ID);
            Assert.True(headers.CacheStatus == CacheStatus.Bypass);


            Assert.Equal(productsIdSumCache, productsIdSumCacheBypass);

        }

        [Fact]
        public void CanClearCache()
        {
            _testService.SetupHttpContext(new DefaultHttpContext() { TraceIdentifier = WipConstants.InternalHttpContextIdentifier });

            var headers = new ApiResponseHeadersModel();
            _productRepository.GetProducts(headers: headers);
            _productRepository.GetProducts(headers: headers);
            Assert.True(headers.CacheStatus == CacheStatus.Hit);

            var client = (IApiClient)_testService.Server.Host.Services.GetService(typeof(IApiClient));
            client.LoginAdmin("wadmin", "wadmin");

            _customerRepository.GetCustomers(headers: headers);
            _customerRepository.GetCustomers(headers: headers);
            Assert.True(headers.CacheStatus == CacheStatus.Hit);

            //delete cache for product, then product cache should be gone but customer cache remain
            var res = client.InternalHttpClient.DeleteAsync("Cache?enovaTypeName=enovabaseproduct").Result;

            _productRepository.GetProducts(headers: headers);
            Assert.True(headers.CacheStatus == CacheStatus.Miss);

            _customerRepository.GetCustomers(headers: headers);
            Assert.True(headers.CacheStatus == CacheStatus.Hit);

            //delete all cache
            res = client.InternalHttpClient.DeleteAsync("Cache").Result;
            _customerRepository.GetCustomers(headers: headers);
            Assert.True(headers.CacheStatus == CacheStatus.Miss);
        }
    }
}
