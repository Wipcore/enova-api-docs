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
    public class ContextParameters : IClassFixture<TestService>
    {
        private readonly TestService _testService;
        private readonly HttpClient _client;

        private const string TestProductIdentifier = "GF_GPU";

        public ContextParameters(TestService testService)
        {
            _testService = testService;
            _client = testService.GetNewClient();
        }

        [Fact()]
        public void DefaultLanguageIsSwedish()//as swedish is the default under marketconfiguration
        {
            var response = _client.GetAsync("products/"+TestProductIdentifier).Result.Content.ReadAsStringAsync().Result;
            var product = JsonConvert.DeserializeObject<IDictionary<string, object>>(response);

            var productName = product["Name"];
            Assert.Equal("Geförse", productName);
        }

        [Theory()]
        [InlineData(new object[] { "da", "Gäförse" })]
        [InlineData(new object[] { "en", "Geforce" })]
        public void LanguageParameterSetsLanguage(string languageIdentifier, string name)
        {
            var url = $"products/{TestProductIdentifier}?Language={languageIdentifier}";
            var response = _client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            var product = JsonConvert.DeserializeObject<IDictionary<string, object>>(response);

            var productName = product["Name"];
            Assert.Equal(name, productName);
        }

        [Fact()]
        public void MarketParameterSetsLanguage()
        {
            var url = $"products/{TestProductIdentifier}?Market=danskjavel";
            var response = _client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            var product = JsonConvert.DeserializeObject<IDictionary<string, object>>(response);

            var productName = product["Name"];
            Assert.Equal("Gäförse", productName);
        }

        [Fact()]
        public void LoginSetsLanguage()
        {
            var client = _testService.AdminClient;//the login wadmin has english as language
            var response = client.GetAsync("products/" + TestProductIdentifier).Result.Content.ReadAsStringAsync().Result;
            var product = JsonConvert.DeserializeObject<IDictionary<string, object>>(response);

            var productName = product["Name"];
            Assert.Equal("Geforce", productName);
        }

        [Theory()]
        [InlineData(new object[] { true, true, true, "Geförse" })]
        [InlineData(new object[] { true, true, false, "Geförse" })]
        [InlineData(new object[] { true, false, false, "Geförse" })]
        [InlineData(new object[] { false, false, false, "Geförse" })]
        [InlineData(new object[] { false, true, true, "Geforce" })]
        [InlineData(new object[] { false, false, true, "Gäförse" })]
        public void LanguagePrioIsParameterThenLoginThenMarketThenDefault(bool setLanguage, bool login, bool setMarket, string result)
        {
            var url = $"products/{TestProductIdentifier}";
            if (setLanguage)
                url += "?Language=" + "sv";
            if (setMarket)
                url += setLanguage ? "&Market=danskjavel" : "?Market=danskjavel";

            if(login)
                _testService.Login(_client);

            var response = _client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            var product = JsonConvert.DeserializeObject<IDictionary<string, object>>(response);

            var productName = product["Name"];
            Assert.Equal(result, productName);
        }
    }
}
