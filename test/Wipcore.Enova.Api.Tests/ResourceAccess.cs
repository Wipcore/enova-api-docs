using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Wipcore.Enova.Api.Tests
{
    public class ResourceAccess : IClassFixture<TestService>
    {
        private readonly TestService _testService;
        private readonly HttpClient _client;

        public ResourceAccess(TestService testService)
        {
            _testService = testService;
            _client = testService.GetNewClient();
        }

        public static IEnumerable<object[]> AllResources => new[]
        {
            // name : needsauth
            new object[] { "attributevalues", false }, new object[] { "carts", true }, new object[] { "companies", false }, new object[] { "countries", false },
            new object[] { "currencies", false }, new object[] { "customergroups", true }, new object[] { "customers", true }, new object[] { "languages", false },
            new object[] { "manufacturers", false }, new object[] { "orders", true }, new object[] { "payments", true }, new object[] { "paymenttypes", false },
            new object[] { "pricelists", true }, new object[] { "products", false }, new object[] { "promotions", true }, new object[] { "sections", false },
            new object[] { "shippingstatuses", false }, new object[] { "shippingtypecosts", false }, new object[] { "shippingtypes", false }, new object[] { "suppliers", false },
            new object[] { "systemsettings", true }, new object[] { "systemtexts", true }, new object[] { "taxationrules", false }, new object[] { "taxes", false },
            new object[] { "warehousecompartments", true }, new object[] { "warehouses", true }, new object[] { "attributetypes", false }
        };

        public static IEnumerable<object[]> AuthResources
        {
            get { return AllResources.Where(x => Convert.ToBoolean(x[1])); }
        }

        public static IEnumerable<object[]> NoAuthResources
        {
            get { return AllResources.Where(x => !Convert.ToBoolean(x[1])); }
        }


        [Fact()]
        public void CanListProducts()
        {
            var response = _client.GetAsync("products").Result;
            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory, MemberData(nameof(AllResources))]
        public void CanListAllResourcesAsAdmin(string resource, bool needsAuth)
        {
            var client = _testService.AdminClient;
            
            var response = client.GetAsync(resource).Result;
            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory, MemberData(nameof(NoAuthResources))]
        public void CanListAllUnsensitiveResourcesWithoutAuth(string resource, bool needsAuth)
        {
            var response = _client.GetAsync(resource).Result;
            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory, MemberData(nameof(AuthResources))]
        public void IsDeniedListingSensitiveResourcesWithoutAuth(string resource, bool needsAuth)
        {
            var access = false;
            try
            {
                access = _client.GetAsync(resource).Result.IsSuccessStatusCode;
            }
            catch
            {
            }
            
            Assert.False(access);
        }

        [Theory()]
        [InlineData("69990001")]
        public void CannotAccessCustomerWithoutAuth(string customerIdentifier)
        {
            var access = false;
            try
            {
                access = _client.GetAsync("customers/"+customerIdentifier).Result.IsSuccessStatusCode;
            }
            catch
            {
            }

            Assert.False(access);
        }

        [Theory()]
        [InlineData("299900011111")]
        public void CannotAccessOrderWithoutAuth(string orderIdentifier)
        {
            var access = false;
            try
            {
                access = _client.GetAsync("orders/" + orderIdentifier).Result.IsSuccessStatusCode;
            }
            catch
            {
            }

            Assert.False(access);
        }

        [Theory()]
        [InlineData(new object[] { "69990001", "69990001", "stygg", true })]
        [InlineData(new object[] { "69990001", "997545", "notstygg", false })]
        public void CanAccessCustomerOnlyAsSameCustomer(string customerIdentifier, string customerAlias, string customerPassword, bool isOwner)
        {
            var access = false;
            _testService.LoginCustomer(_client, customerAlias, customerPassword);
            try
            {
                access = _client.GetAsync("customers/" + customerIdentifier).Result.IsSuccessStatusCode;
            }
            catch
            {
            }

            if (isOwner)
                Assert.True(access);
            else
                Assert.False(access);
        }

        [Theory()]
        [InlineData(new object[] {"299900011111", "69990001", "stygg", true })]
        [InlineData(new object[] { "299900011111", "997545", "notstygg", false })]
        public void CanAccessOrderOnlyAsCustomerOwningOrder(string orderIdentifier, string customerAlias, string customerPassword, bool isOwner)
        {
            var access = false;
            _testService.LoginCustomer(_client, customerAlias, customerPassword);
            try
            {
                access = _client.GetAsync("orders/" + orderIdentifier).Result.IsSuccessStatusCode;
            }
            catch
            {
            }

            if(isOwner)
                Assert.True(access);
            else
                Assert.False(access);
        }
    }
}
