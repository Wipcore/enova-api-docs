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
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product;
using Wipcore.Enova.Api.NetClient;
using Wipcore.Enova.Api.NETClient;
using Wipcore.Enova.Api.WebApi;
using Wipcore.Enova.Connectivity;
using Xunit;
using CartModel = Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart.CartModel;
using CustomerModel = Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer.CustomerModel;

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
        private static int _testClasses = 0;
        private static TestServer _server;
        private static HttpClient _adminClient;

        public static IConfigurationRoot Configuration { get; set; }

        public TestServer Server => _server;

        public HttpClient AdminClient => _adminClient;

        public IApiClient ApiClient { get; set; }

        public TestService()
        {
            lock (Lock)
            {
                _testClasses++;
                if (Server != null)
                    return;

                try
                {


                    var builder = new ConfigurationBuilder().AddJsonFile(@"Configs\appsettings.json").AddJsonFile(@"Configs\localappsettings.json", true);
                    Configuration = builder.Build();
                    _server = new TestServer(new WebHostBuilder().UseStartup<Startup>().ConfigureServices(ConfigureTestServices()));
                    _adminClient = GetNewClient(true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            
        }

        private Action<IServiceCollection> ConfigureTestServices()
        {
            return x =>
            {
                x.AddTransient(typeof(IApiClient), s => //NOTE only used for repository tests
                    {
                        if (ApiClient != null)
                        {
                            //if the end user has a token cookie, then place the token in the header for requests made by this client
                            var accessor = (IHttpContextAccessor)s.GetService(typeof(IHttpContextAccessor));
                            var tokenCookie = accessor.HttpContext.Request.Cookies["ApiToken"];
                            if (!String.IsNullOrEmpty(tokenCookie))
                            {
                                ApiClient.InternalHttpClient.DefaultRequestHeaders.Remove("Authorization");
                                ApiClient.InternalHttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenCookie);
                            }

                            return ApiClient;
                        }

                        ApiClient = new ApiClient(new ApiClientAsync(s.GetService<IConfigurationRoot>(),
                            s.GetService<IHttpContextAccessor>(), s.GetService<IDataProtectionProvider>(),
                            s.GetService<ILoggerFactory>()));

                        var serverClient = _server.CreateClient();

                        serverClient.BaseAddress = new Uri(Configuration["API:Url"] ?? "http://localhost:5000/api/");
                        ApiClient.InternalHttpClient = serverClient;

                        return ApiClient;
                    }
                );
                x.AddSingleton(typeof(IApiRepository), typeof(ApiRepository));
                x.AddTransient(typeof(CustomerModel));
                x.AddTransient(typeof(CartModel));
                x.AddTransient(typeof(OrderModel));
                x.AddTransient(typeof(ProductModel));
                x.AddSingleton(typeof(CustomerRepository< CustomerModel, CartModel, OrderModel >));
                x.AddSingleton(typeof(CartRepository<CartModel, OrderModel>));
                x.AddSingleton(typeof(OrderRepository<OrderModel>));
                x.AddSingleton(typeof(ProductRepository<ProductModel>));
            };
        }

        public void Dispose()
        {
            lock (Lock)
            {
                _testClasses--;
                if (_testClasses == 0)
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
            client.DefaultRequestHeaders.Remove("Authorization");
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
