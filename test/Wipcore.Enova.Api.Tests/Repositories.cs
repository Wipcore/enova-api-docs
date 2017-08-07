using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Wipcore.eNova.Api.NETClient;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;
using Xunit;

namespace Wipcore.Enova.Api.Tests
{
    [Collection("WebApiCollection")]
    public class Repositories : IClassFixture<TestService>
    {
        private readonly TestService _testService;
        private readonly CustomerRepository<CustomerModel, CartModel, OrderModel> _customerRepository;
        private readonly Random _random = new Random();
        private IApiClient _apiClient;

        public Repositories(TestService testService)
        {
            _testService = testService;
            _customerRepository = (CustomerRepository<CustomerModel, CartModel, OrderModel>)_testService.Server.Host.Services.GetService(typeof(CustomerRepository<CustomerModel, CartModel, OrderModel>));

        }

        private void SetupHttpContext(HttpContext context)
        {
            var accessor = (IHttpContextAccessor)_testService.Server.Host.Services.GetService(typeof(IHttpContextAccessor));
            accessor.HttpContext = context;
        }

        [Theory]
        [InlineData(new object[] { "69990002", "password" })]
        public void CanUpdateCustomerByRepo(string customerIdentifier, string password)
        {
            SetupHttpContext(new DefaultHttpContext() { TraceIdentifier = WipConstants.ElasticDeltaIndexHttpContextIdentifier });

            _customerRepository.LoginCustomer(customerIdentifier, password);

            var newSortOrder = _random.Next(1000);
            var model = new CustomerModel() { Identifier = customerIdentifier, SortOrder = newSortOrder, Alias = customerIdentifier };

            _customerRepository.CreateOrUpdateCustomer(model, verifyIdentifierNotTaken: false);
            var savedModel = _customerRepository.GetSavedCustomer(customerIdentifier);
            Assert.Equal(newSortOrder, savedModel.SortOrder);
        }

        [Theory]
        [InlineData(new object[] { "69990002", "password" })]
        public void CanGetCustomerCartsByRepo(string customerIdentifier, string password)
        {
            SetupHttpContext(new DefaultHttpContext() { TraceIdentifier = WipConstants.ElasticDeltaIndexHttpContextIdentifier });

            _customerRepository.LoginCustomer(customerIdentifier, password);
            var customer = _customerRepository.GetSavedCustomer(customerIdentifier);
            
            var carts2 = _customerRepository.GetCarts(customer.Identifier);
            var carts = _customerRepository.GetCarts(customer.ID);

            Assert.NotEqual(carts.Count, 0);
            carts.ForEach(x => Assert.NotEqual(x.ID, 0));

            Assert.NotEqual(carts2.Count, 0);
            carts2.ForEach(x => Assert.NotEqual(x.ID, 0));

            Assert.Equal(carts.Count, carts2.Count);
        }

    }
}
