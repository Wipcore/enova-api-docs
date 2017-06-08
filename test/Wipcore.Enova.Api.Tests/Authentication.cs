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
    public class Authentication : IClassFixture<TestService>
    {
        private readonly TestService _testService;
        private readonly HttpClient _client;

        public Authentication(TestService testService)
        {
            _testService = testService;
            _client = testService.GetNewClient();
        }

        [Theory]
        [InlineData(new object[] { "69990001", "notARealPassword", false })]
        [InlineData(new object[] { "69990001", "stygg", true})]
        [InlineData(new object[] { "997545", "notstygg", true })]
        public void CanLoginBeLoggedInAndLogout(string customerAlias, string customerPassword, bool validLogin)
        {
            _testService.LoginCustomer(_client, customerAlias, customerPassword, validLogin);

            var response = _client.GetAsync("/Account/LoggedInAs").Result;
            response.EnsureSuccessStatusCode();

            var loggedInInfo = JsonConvert.DeserializeObject<IDictionary<string, string>>(response.Content.ReadAsStringAsync().Result);

            var loggedIn = Convert.ToBoolean(loggedInInfo["LoggedIn"]);

            //verify successfull login, or failed on wrong pass
            Assert.Equal(validLogin, loggedIn);

            if(loggedIn)//verify that if logged in, logged in as the right user
                Assert.Equal(customerAlias, loggedInInfo["Alias"]);

            //logout
            response = _client.PostAsync("/Account/Logout", null).Result;
            response.EnsureSuccessStatusCode();

            //ask again if loggedin
            response = _client.GetAsync("/Account/LoggedInAs").Result;
            response.EnsureSuccessStatusCode();

            loggedInInfo = JsonConvert.DeserializeObject<IDictionary<string, string>>(response.Content.ReadAsStringAsync().Result);
            loggedIn = Convert.ToBoolean(loggedInInfo["LoggedIn"]);

            //Assert.False(loggedIn);//should never be logged in after a logout. TODO current netcore does not actually implement removing bearer token...
        }

    }
}
