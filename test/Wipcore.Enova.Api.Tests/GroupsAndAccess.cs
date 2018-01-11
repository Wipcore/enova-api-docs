using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Administrator;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;
using Wipcore.Enova.Api.NetClient;
using Wipcore.Enova.Api.NetClient.Administrator;
using Wipcore.Enova.Api.NetClient.Customer;
using Xunit;

namespace Wipcore.Enova.Api.Tests
{
    [Collection("WebApiCollection")]
    public class GroupsAndAccess : IClassFixture<TestService>
    {
        private readonly TestService _testService;
        private readonly HttpClient _client;
        private readonly CustomerRepository<CustomerModel, CartModel, OrderModel> _customerRepository;
        private readonly AdministratorRepository<AdministratorModel> _adminRepository;

        public GroupsAndAccess(TestService testService)
        {
            _testService = testService;
            _client = testService.GetNewClient();
            _customerRepository = (CustomerRepository<CustomerModel, CartModel, OrderModel>)_testService.Server.Host.Services.GetService(typeof(CustomerRepository<CustomerModel, CartModel, OrderModel>));
            _adminRepository = (AdministratorRepository<AdministratorModel>)_testService.Server.Host.Services.GetService(typeof(AdministratorRepository<AdministratorModel>));
        }

        [Theory]
        [InlineData(new object[] { "69990001", "DEFAULT" })]
        public void CanAddAndRemoveCustomerFromGroup(string customerIdentifier, string customerGroupIdentifier)
        {
            _testService.SetupHttpContext(new DefaultHttpContext() { TraceIdentifier = WipConstants.ElasticDeltaIndexHttpContextIdentifier });

            _adminRepository.LoginAdmin("wadmin", "wadmin");

            //remove, to start clean
            _customerRepository.RemoveCustomerFromGroup(customerIdentifier, customerGroupIdentifier);

            var groups = _customerRepository.GetGroupsForCustomer(customerIdentifier).Where(x => String.Equals(x.Identifier, customerGroupIdentifier, StringComparison.InvariantCultureIgnoreCase));
            Assert.Empty(groups);

            //add
            _customerRepository.AddCustomerToGroup(customerIdentifier, customerGroupIdentifier);

            //check it exists
            groups = _customerRepository.GetGroupsForCustomer(customerIdentifier).Where(x => String.Equals(x.Identifier, customerGroupIdentifier, StringComparison.InvariantCultureIgnoreCase));
            var customers = _customerRepository.GetCustomersForGroup(customerGroupIdentifier).Where(x => String.Equals(x.Identifier, customerIdentifier, StringComparison.InvariantCultureIgnoreCase));

            Assert.Single(groups);
            Assert.Single(customers);

            //remove, check it's now empty
            _customerRepository.RemoveCustomerFromGroup(customerIdentifier, customerGroupIdentifier);

            groups = _customerRepository.GetGroupsForCustomer(customerIdentifier).Where(x => String.Equals(x.Identifier, customerGroupIdentifier, StringComparison.InvariantCultureIgnoreCase));
            customers = _customerRepository.GetCustomersForGroup(customerGroupIdentifier).Where(x => String.Equals(x.Identifier, customerIdentifier, StringComparison.InvariantCultureIgnoreCase));

            Assert.Empty(groups);
            Assert.Empty(customers);
        }

    }
}
