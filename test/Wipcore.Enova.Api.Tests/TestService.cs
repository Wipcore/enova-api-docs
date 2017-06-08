using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.WebApi;
using Wipcore.Enova.Connectivity;
using Xunit;

namespace Wipcore.Enova.Api.Tests
{
    [CollectionDefinition("WebApiCollection")]
    public class EnovaApiAccessCollection : ICollectionFixture<TestService>, IDisposable
    {
        public void Dispose() { }
    }

    [Collection("WebApiCollection")]
    public class TestService : IDisposable
    {
        private static readonly object Lock = new object();//Locking as workaround for test collections not working
        private static int TestClasses = 0;
        private static TestServer _server;
        private static HttpClient _adminClient;

        public static IConfigurationRoot Configuration { get; set; }

        public TestServer Server => _server;

        public HttpClient AdminClient => _adminClient;

        public TestService()
        {
            lock (Lock)
            {
                TestClasses++;
                if (Server != null)
                    return;

                try
                {
                    var builder = new ConfigurationBuilder().AddJsonFile(@"Configs\appsettings.json").AddJsonFile(@"Configs\localappsettings.json", true);
                    Configuration = builder.Build();
                    _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                    _adminClient = GetNewClient(true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            
        }

        public void Dispose()
        {
            lock (Lock)
            {
                TestClasses--;
                if (TestClasses == 0)
                {
                    _server.Dispose();
                    _server = null;
                }
                    
            }
            
        }

        public HttpClient GetNewClient(bool login = false)
        {
            var client = Server.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5000/api/");

            if(login)
                Login(client);

            return client;
        }

        public void Login(HttpClient client)
        {
            var model = new LoginModel() { Alias = Configuration["Enova:Username"], Password = Configuration["Enova:Password"] };
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, new UTF8Encoding(), "application/json");

            var response = client.PostAsync("/Account/LoginAdmin", content).Result;
            Assert.True(response.IsSuccessStatusCode, "Failed to login admin.");

            var loginResponse = JsonConvert.DeserializeObject<LoginResponseModel>(response.Content.ReadAsStringAsync().Result);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + loginResponse.AccessToken);
        }

        public void LoginCustomer(HttpClient client, string alias, string password, bool loginShouldBeValid = true)
        {
            var model = new LoginModel() { Alias = alias, Password = password };
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, new UTF8Encoding(), "application/json");

            var response = client.PostAsync("/Account/LoginCustomer", content).Result;

            if (loginShouldBeValid)
            {
                Assert.True(response.IsSuccessStatusCode, $"Failed to login customer with alias: {alias} and password: {password}");

                var loginResponse = JsonConvert.DeserializeObject<LoginResponseModel>(response.Content.ReadAsStringAsync().Result);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + loginResponse.AccessToken);
            }
            else
            {
                Assert.False(response.IsSuccessStatusCode);    
            }
        }
    }
}
