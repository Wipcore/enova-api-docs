using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Wipcore.Enova.Api.Tests
{
    [Collection("WebApiCollection")]
    public class CreateUpdateDelete: IClassFixture<TestService>
    {
        private readonly TestService _testService;
        private readonly HttpClient _client;
        private static readonly Random Rand = new Random();

        private const string TestBaseIdentifier = "_Test";

        public CreateUpdateDelete(TestService testService)
        {
            _testService = testService;
            _client = testService.GetNewClient();
        }

        public static IEnumerable<object[]> AllUpdateableResources => new[]
        {
            // name : needsauth
            new object[] { "carts", true }, new object[] { "companies", false }, new object[] { "customergroups", true }, new object[] { "customers", true },
            new object[] { "manufacturers", false }, new object[] { "orders", true }, new object[] { "payments", true }, new object[] { "paymenttypes", false },
            new object[] { "pricelists", true }, new object[] { "products", false }, new object[] { "promotions", true }, new object[] { "sections", false },
            new object[] { "shippingstatuses", false }, new object[] { "shippingtypes", false }, 
            new object[] { "systemsettings", true }, new object[] { "systemtexts", true }, new object[] { "attributetypes", false }
        };


        [Theory, MemberData(nameof(AllUpdateableResources))]
        public void CanCrudResourceAsAdmin(string resource, bool auth)
        {
            var identifier = resource + TestBaseIdentifier;

            DeleteResource(resource, identifier, _testService.AdminClient);//clear any existing
            
            var model = new Dictionary<string, object>() {{"Identifier", identifier}, { "SortOrder", 0}};
            if(resource == "customers")
                model.Add("Alias", identifier);

            var createdResource = UpdateOrCreateResource(resource, _testService.AdminClient, model);//create
            Assert.NotNull(createdResource);

            var newRandomSortOrder = Rand.Next();
            model["SortOrder"] = newRandomSortOrder;
            var updatedResource = UpdateOrCreateResource(resource, _testService.AdminClient, model);//update
            
            Assert.NotNull(updatedResource);
            Assert.Equal(newRandomSortOrder, Convert.ToInt32(updatedResource["SortOrder"]));

            DeleteResource(resource, identifier, _testService.AdminClient);
            Assert.Null(GetResource(resource, identifier, _testService.AdminClient));
        }

        [Theory, MemberData(nameof(AllUpdateableResources))]
        public void CannotCrudResourceWithoutAuth(string resource, bool auth)
        {
            var identifier = resource + TestBaseIdentifier;

            DeleteResource(resource, identifier, _testService.AdminClient); //clear any existing

            var model = new Dictionary<string, object>() { { "Identifier", identifier } };
            if (resource == "customers")
                model.Add("Alias", identifier);

            //create using non admin client, should fail
            UpdateOrCreateResource(resource, _client, model);
            Assert.Null(GetResource(resource, identifier, _testService.AdminClient));

            //create with admin client and then delete with normal client, should also fail
            var createdResource = UpdateOrCreateResource(resource, _testService.AdminClient, model);
            Assert.NotNull(createdResource);
            DeleteResource(resource, identifier, _client);
            Assert.NotNull(GetResource(resource, identifier, _testService.AdminClient));
        }

        [Theory, MemberData(nameof(AllUpdateableResources))]
        public void CannotCrudResourceAsCustomer(string resource, bool auth)
        {
            if(resource == "orders" || resource == "carts")
                return; //allowed

            _testService.LoginCustomer(_client, "69990001", "stygg");
            CannotCrudResourceWithoutAuth(resource, auth);
        }

        [Theory]
        [InlineData(new object[] { "69990001", "69990001", "stygg", "29990005", "testcart", true })]
        [InlineData(new object[] { "69990001", "997545", "notstygg", "29990005", "testcart", false })]
        public void CustomerCanCruItself(string customerIdentifier, string customerAlias, string customerPassword, string orderIdentifier, string cartIdentifier, bool isOwner)
        {
            _testService.LoginCustomer(_client, customerAlias, customerPassword);

            //update customer
            var newRandomSortOrder = Rand.Next();
            var model = new Dictionary<string, object>() { { "Identifier", customerIdentifier }, {"SortOrder", newRandomSortOrder} };
            var updatedModel = UpdateOrCreateResource("customers", _client, model);

            if (isOwner)
                Assert.Equal(newRandomSortOrder, Convert.ToInt32(updatedModel["SortOrder"]));
            else
                Assert.Null(updatedModel);

            //update cart
            newRandomSortOrder = Rand.Next();
            model = new Dictionary<string, object>() { { "Identifier", cartIdentifier }, { "SortOrder", newRandomSortOrder } };
            updatedModel = UpdateOrCreateResource("carts", _client, model);

            if (isOwner)
                Assert.Equal(newRandomSortOrder, Convert.ToInt32(updatedModel["SortOrder"]));
            else
                Assert.Null(updatedModel);

            //update order
            newRandomSortOrder = Rand.Next();
            model = new Dictionary<string, object>() { { "Identifier", orderIdentifier }, { "SortOrder", newRandomSortOrder } };
            updatedModel = UpdateOrCreateResource("orders", _client, model);

            if (isOwner)
                Assert.Equal(newRandomSortOrder, Convert.ToInt32(updatedModel["SortOrder"]));
            else
                Assert.Null(updatedModel);

            //delete should never work
            Assert.False(DeleteResource("orders", orderIdentifier, _client));
            Assert.False(DeleteResource("carts", cartIdentifier, _client));
            Assert.False(DeleteResource("customers", customerIdentifier, _client));
        }


        private IDictionary<string, object> GetResource(string resource, string identifier, HttpClient client)
        {
            try
            {
                var url = $"{resource}/{identifier}?properties=Identifier,SortOrder";
                var response = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<IDictionary<string, object>>(response);

                return item;
            }
            catch
            {
                return null;
            }
        }

        private IDictionary<string, object> UpdateOrCreateResource(string resource, HttpClient client, IDictionary<string, object> model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, new UTF8Encoding(), "application/json");

                var url = $"{resource}";
                var response = client.PutAsync(url, content).Result.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<IDictionary<string, object>>(response);

                return item;
            }
            catch
            {
                return null;
            }
        }

        private bool DeleteResource(string resource, string identifier, HttpClient client)
        {
            try
            {
                var url = $"{resource}/{identifier}";
                var response = client.DeleteAsync(url).Result;
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
