using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;
using CartModel = Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart.CartModel;
using CustomerModel = Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer.CustomerModel;

namespace Wipcore.Enova.Api.NetClient.Customer
{
    public class CustomerRepositoryAsync<TCustomerModel, TCartModel, TOrderModel> 
        where TCartModel : CartModel where TOrderModel : OrderModel where TCustomerModel : CustomerModel
    {
        private readonly IApiRepositoryAsync _apiRepository;
        private readonly Func<IApiClientAsync> _apiClient;

        public CustomerRepositoryAsync(IApiRepositoryAsync apiRepository, Func<IApiClientAsync> apiClient)
        {
            _apiRepository = apiRepository;
            _apiClient = apiClient;
        }

        public async Task<bool> IsLoggedIn()
        {
            var loggedInInfo = await _apiClient.Invoke().GetLoggedInUserInfoAsync().ConfigureAwait(WipConstants.ContinueOnCapturedContext);
            return Convert.ToBoolean(loggedInInfo["LoggedIn"]);
        }

        public async Task<bool> IsLoggedInAsCustomer()
        {
            var loggedInInfo = await _apiClient.Invoke().GetLoggedInUserInfoAsync().ConfigureAwait(WipConstants.ContinueOnCapturedContext);
            return Convert.ToBoolean(loggedInInfo["LoggedIn"]) && loggedInInfo["Role"] == "customer";
        }

        public async Task<bool> IsLoggedInAsAdmin()
        {
            var loggedInInfo = await _apiClient.Invoke().GetLoggedInUserInfoAsync().ConfigureAwait(WipConstants.ContinueOnCapturedContext);
            return Convert.ToBoolean(loggedInInfo["LoggedIn"]) && loggedInInfo["Role"] == "admin";
        }

        public async Task<IDictionary<string, string>> GetLoggedInUserInfo()
        {
            return await _apiClient.Invoke().GetLoggedInUserInfoAsync().ConfigureAwait(WipConstants.ContinueOnCapturedContext);
        }

        public async Task<ILoginResponseModel> LoginAdmin(string alias, string password) =>
            await _apiClient.Invoke().LoginAdminAsync(alias, password).ConfigureAwait(WipConstants.ContinueOnCapturedContext);

        public async Task<ILoginResponseModel> LoginCustomer(string alias, string password) => 
            await _apiClient.Invoke().LoginCustomerAsync(alias, password).ConfigureAwait(WipConstants.ContinueOnCapturedContext);

        public async Task<ILoginResponseModel> LoginCustomerAsAdmin(string customerAlias, string adminAlias, string adminPassword) => 
            await _apiClient.Invoke().LoginCustomerAsAdminAsync(customerAlias, adminAlias, adminPassword).ConfigureAwait(WipConstants.ContinueOnCapturedContext);


        public async Task<TCustomerModel> GetSavedCustomer(int id, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return await _apiRepository.GetObjectAsync<TCustomerModel>(id, queryModel, contextModel).ConfigureAwait(WipConstants.ContinueOnCapturedContext);
        }

        public async Task<TCustomerModel> GetSavedCustomer(string identifier, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return await _apiRepository.GetObjectAsync<TCustomerModel>(identifier, queryModel, contextModel).ConfigureAwait(WipConstants.ContinueOnCapturedContext);
        }

        public async Task<TCustomerModel> CreateCustomer(TCustomerModel customer, ContextModel contextModel = null)
        {
            return (TCustomerModel)await _apiRepository.SaveObjectAsync<TCustomerModel>(JObject.FromObject(customer), contextModel: contextModel, verifyIdentifierNotTaken: true).ConfigureAwait(WipConstants.ContinueOnCapturedContext);
        }

        public async Task<TCustomerModel> UpdateCustomer(TCustomerModel customer, ContextModel contextModel = null)
        {
            return (TCustomerModel)await _apiRepository.SaveObjectAsync<TCustomerModel>(JObject.FromObject(customer), contextModel: contextModel, verifyIdentifierNotTaken: false).ConfigureAwait(WipConstants.ContinueOnCapturedContext);
        }

        public async Task<List<TCartModel>> GetCarts(int customerId, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return (await _apiRepository.GetManyAsync<TCartModel>(queryModel, headers, contextModel, $"ofcustomerid-{customerId}").ConfigureAwait(WipConstants.ContinueOnCapturedContext)).ToList();
        }

        public async Task<List<TCartModel>> GetCarts(string customerIdentifier, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return (await _apiRepository.GetManyAsync<TCartModel>(queryModel, headers, contextModel, $"ofcustomer-{customerIdentifier}").ConfigureAwait(WipConstants.ContinueOnCapturedContext)).ToList();
        }

        public async Task<List<TOrderModel>> GetOrders(int customerId, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null, string shippingStatusIdentifier = null)
        {
            var statusFilter = String.IsNullOrEmpty(shippingStatusIdentifier) ? null : new Dictionary<string, object>() { { "shippingStatus", shippingStatusIdentifier } };

            return (await _apiRepository.GetManyAsync<TOrderModel>(queryModel, headers, contextModel, $"ofcustomerid-{customerId}", extraParameters: statusFilter).ConfigureAwait(WipConstants.ContinueOnCapturedContext)).ToList();
        }

        public async Task<List<TOrderModel>> GetOrders(string customerIdentifier, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null, string shippingStatusIdentifier = null)
        {
            var statusFilter = String.IsNullOrEmpty(shippingStatusIdentifier) ? null : new Dictionary<string, object>() { { "shippingStatus", shippingStatusIdentifier } };

            return (await _apiRepository.GetManyAsync<TOrderModel>(queryModel, headers, contextModel, $"ofcustomer-{customerIdentifier}", extraParameters:statusFilter).ConfigureAwait(WipConstants.ContinueOnCapturedContext)).ToList();
        }

        public async Task<bool> DeleteCustomer(string customerIdentifier) => await _apiRepository.DeleteObjectAsync<TCustomerModel>(customerIdentifier).ConfigureAwait(WipConstants.ContinueOnCapturedContext);

        public async Task<bool> DeleteCustomer(int customerId) => await _apiRepository.DeleteObjectAsync<TCustomerModel>(customerId).ConfigureAwait(WipConstants.ContinueOnCapturedContext);

        public async Task<List<GroupMiniModel>> GetGroupsForCustomer(string customerIdentifier, QueryModel queryModel = null, ContextModel contextModel = null)
            => (await _apiRepository.GetObjectAsync<TCustomerModel>(customerIdentifier, queryModel, contextModel).ConfigureAwait(WipConstants.ContinueOnCapturedContext)).Groups;

        public async Task<List<GroupMiniModel>> GetGroupsForCustomer(int customerId, QueryModel queryModel = null, ContextModel contextModel = null)
            => (await _apiRepository.GetObjectAsync<TCustomerModel>(customerId, queryModel, contextModel).ConfigureAwait(WipConstants.ContinueOnCapturedContext)).Groups;

        public async Task<List<CustomerMiniModel>> GetCustomersForGroup(string groupIdentifier, ContextModel contextModel = null)
            => (await _apiRepository.GetObjectAsync<CustomerGroupModel>(groupIdentifier, null, contextModel).ConfigureAwait(WipConstants.ContinueOnCapturedContext)).Users;

        public async Task<List<CustomerMiniModel>> GetCustomersForGroup(int groupId, ContextModel contextModel = null)
            => (await _apiRepository.GetObjectAsync<CustomerGroupModel>(groupId, null, contextModel).ConfigureAwait(WipConstants.ContinueOnCapturedContext)).Users;

        public async Task AddCustomerToGroup(string customerIdentifier, string groupIdentifier)
        {
            var customerGroup = new CustomerGroupModel() { Identifier = groupIdentifier, Users = new List<CustomerMiniModel>() { new CustomerMiniModel() { Identifier = customerIdentifier } } };
            var json = JObject.FromObject(customerGroup);
            await _apiRepository.SaveObjectAsync<CustomerGroupModel>(json, verifyIdentifierNotTaken: false).ConfigureAwait(WipConstants.ContinueOnCapturedContext);
        }

        public async Task AddCustomerToGroup(int customerId, int groupId)
        {
            var customerGroup = new CustomerGroupModel() { ID = groupId, Users = new List<CustomerMiniModel>() { new CustomerMiniModel() { ID = customerId } } };
            var json = JObject.FromObject(customerGroup);
            await _apiRepository.SaveObjectAsync<CustomerGroupModel>(json, verifyIdentifierNotTaken: false).ConfigureAwait(WipConstants.ContinueOnCapturedContext);
        }

        public async Task RemoveCustomerFromGroup(string customerIdentifier, string groupIdentifier)
        {
            var customerGroup = new CustomerGroupModel() { Identifier = groupIdentifier, Users = new List<CustomerMiniModel>() { new CustomerMiniModel() { Identifier = customerIdentifier, MarkForDelete = true } } };
            var json = JObject.FromObject(customerGroup);
            await _apiRepository.SaveObjectAsync<CustomerGroupModel>(json, verifyIdentifierNotTaken: false).ConfigureAwait(WipConstants.ContinueOnCapturedContext);
        }

        public async Task RemoveCustomerFromGroup(int customerId, int groupId)
        {
            var customerGroup = new CustomerGroupModel() { ID = groupId, Users = new List<CustomerMiniModel>() { new CustomerMiniModel() { ID = customerId, MarkForDelete = true } } };
            var json = JObject.FromObject(customerGroup);
            await _apiRepository.SaveObjectAsync<CustomerGroupModel>(json, verifyIdentifierNotTaken: false).ConfigureAwait(WipConstants.ContinueOnCapturedContext);
        }

    }
}
