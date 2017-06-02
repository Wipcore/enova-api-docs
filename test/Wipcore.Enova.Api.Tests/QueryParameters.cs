using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Wipcore.Enova.Api.Tests
{
    [Collection("WebApiCollection")]
    public class QueryParameters : IClassFixture<TestService>
    {
        private readonly TestService _testService;
        private readonly HttpClient _client;

        public QueryParameters(TestService testService)
        {
            _testService = testService;
            _client = testService.GetNewClient();
        }

        [Fact()]
        public void CanFilterOnName()
        {
            var filterValue = "Geforce";

            var url = $"products?filter=Name={filterValue}*&language=en";
            var response = _client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            var products = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(response).ToList();

            Assert.Equal(4, products.Count);

            foreach (var product in products)
            {
                Assert.StartsWith(filterValue, product["Name"].ToString());
            }
        }

        [Theory]
        [InlineData(10)]
        [InlineData(40)]
        public void CanSetResponseSize(int size)
        {
            var url = $"products?size={size}";
            var response = _client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            var products = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(response).ToList();

            Assert.Equal(size, products.Count);
        }

        [Fact]
        public void CanPageTheResponse()
        {
            //get all products in one request
            var url = $"products?properties=ID&size={1000000}";
            var response = _client.GetAsync(url).Result;
            var allProductIds = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(response.Content.ReadAsStringAsync().Result).Select(x => x["ID"]).ToList();

            var totalRecordCountAccordingToHeader = 0;
            IEnumerable<string> values;
            if (response.Headers.TryGetValues("X-Paging-TotalRecordCount", out values))
                totalRecordCountAccordingToHeader = Convert.ToInt32(values.First());

            Assert.Equal(allProductIds.Count, totalRecordCountAccordingToHeader);

            //Get all IDs 20 at a time
            var pageinationIds = new List<int>();
            var pageSize = 20;
            var pageCount = (allProductIds.Count + pageSize - 1) / pageSize;
            for (int i = 1; i <= pageCount; i++)
            {
                url = $"products?page={i}&properties=ID&size={pageSize}";
                var subResponse = _client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
                var products = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(subResponse).ToList();

                pageinationIds.AddRange(products.Select(x => Convert.ToInt32(x["ID"])));
            }

            pageinationIds = pageinationIds.Distinct().ToList();
            
            Assert.Equal(allProductIds.Count, pageinationIds.Count);
        }

        [Theory]
        [InlineData(new object[] { "ID" })]
        [InlineData(new object[] { "ID", "Identifier" })]
        [InlineData(new object[] { "ID", "Identifier", "Name" })]
        [InlineData(new object[] { "Name", "ExternalIdentifier" })]
        public void CanGetCustomProperties(params string[] properties)
        {
            var propertiesInUrl = String.Join(",", properties);
            var url = $"products?properties={propertiesInUrl}&size=10";
            var response = _client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            var products = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(response).ToList();

            foreach (var product in products)
            {
                Assert.Equal(properties.Length, product.Count);//correct number of properties
                foreach (var property in properties)
                {
                    Assert.True(product.ContainsKey(property));//has requested property
                }
            }
        }

        [Fact]
        public void CanGetPropertiesByTemplate()
        {
            var propertiesInTemplate = "Identifier,PriceExclTax,PriceInclTax,IsDiscounted,ListPriceInclTax,ListPriceExclTax".Split(',');
            var url = "products?Template=Price&size=5";
            var response = _client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            var products = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(response).ToList();

            foreach (var product in products)
            {
                Assert.Equal(propertiesInTemplate.Length, product.Count);//correct number of properties
                foreach (var property in propertiesInTemplate)
                {
                    Assert.True(product.ContainsKey(property));//has requested property
                }
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanSortAscAndDesc(bool desc)
        {
            var url = "products?properties=ID&sort=ID";
            if (desc)
                url += " desc";

            var response = _client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            var products = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(response).ToList();

            for (int i = 1; i < products.Count; i++)
            {
                var previousId = Convert.ToInt32(products[i - 1]["ID"]);
                var id = Convert.ToInt32(products[i]["ID"]);

                if(desc)
                    Assert.True(previousId > id);
                else
                    Assert.True(previousId < id);
            }
        }
    }
}
