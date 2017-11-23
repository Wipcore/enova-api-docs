using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;
using CartModel = Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart.CartModel;
using CustomerModel = Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer.CustomerModel;

namespace Wipcore.Enova.Api.NetClient
{
    public class UserRepositoryAsync<TCustomerModel, TCartModel, TOrderModel> where TCartModel : CartModel where TOrderModel : OrderModel where TCustomerModel : CustomerModel
    {
        private readonly IApiRepositoryAsync _apiRepository;
        private readonly Func<IApiClientAsync> _apiClient;

        public UserRepositoryAsync(IApiRepositoryAsync apiRepository, Func<IApiClientAsync> apiClient)
        {
            _apiRepository = apiRepository;
            _apiClient = apiClient;
        }

        public async Task<bool> IsLoggedIn()
        {
            var loggedInInfo = await _apiClient.Invoke().GetLoggedInUserInfoAsync();
            return Convert.ToBoolean(loggedInInfo["LoggedIn"]);
        }

        public async Task<bool> IsLoggedInAsCustomer()
        {
            var loggedInInfo = await _apiClient.Invoke().GetLoggedInUserInfoAsync();
            return Convert.ToBoolean(loggedInInfo["LoggedIn"]) && loggedInInfo["Role"] == "customer";
        }

        public async Task<bool> IsLoggedInAsAdmin()
        {
            var loggedInInfo = await _apiClient.Invoke().GetLoggedInUserInfoAsync();
            return Convert.ToBoolean(loggedInInfo["LoggedIn"]) && loggedInInfo["Role"] == "admin";
        }

        public async Task<IDictionary<string, string>> GetLoggedInUserInfo()
        {
            return await _apiClient.Invoke().GetLoggedInUserInfoAsync();
        }

        public async Task<ILoginResponseModel> LoginAdmin(string alias, string password) =>
            await _apiClient.Invoke().LoginAdminAsync(alias, password);

        public async Task<ILoginResponseModel> LoginCustomer(string alias, string password) => 
            await _apiClient.Invoke().LoginCustomerAsync(alias, password);

        public async Task<ILoginResponseModel> LoginCustomerAsAdmin(string customerAlias, string adminAlias, string adminPassword) => 
            await _apiClient.Invoke().LoginCustomerAsAdminAsync(customerAlias, adminAlias, adminPassword);


        public async Task<TCustomerModel> GetSavedCustomer(int id, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return await _apiRepository.GetObjectAsync<TCustomerModel>(id, queryModel, contextModel);
        }

        public async Task<TCustomerModel> GetSavedCustomer(string identifier, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return await _apiRepository.GetObjectAsync<TCustomerModel>(identifier, queryModel, contextModel);
        }

        public async Task<TCustomerModel> CreateCustomer(TCustomerModel customer, ContextModel contextModel = null)
        {
            return (TCustomerModel)await _apiRepository.SaveObjectAsync<TCustomerModel>(JObject.FromObject(customer), contextModel: contextModel, verifyIdentifierNotTaken: true);
        }

        public async Task<TCustomerModel> UpdateCustomer(TCustomerModel customer, ContextModel contextModel = null)
        {
            return (TCustomerModel)await _apiRepository.SaveObjectAsync<TCustomerModel>(JObject.FromObject(customer), contextModel: contextModel, verifyIdentifierNotTaken: false);
        }

        public async Task<List<TCartModel>> GetCarts(int customerId, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return (await _apiRepository.GetManyAsync<TCartModel>(queryModel, headers, contextModel, $"ofcustomerid-{customerId}")).ToList();
        }

        public async Task<List<TCartModel>> GetCarts(string customerIdentifier, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return (await _apiRepository.GetManyAsync<TCartModel>(queryModel, headers, contextModel, $"ofcustomer-{customerIdentifier}")).ToList();
        }

        public async Task<List<TOrderModel>> GetOrders(int customerId, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null, string shippingStatusIdentifier = null)
        {
            var statusFilter = String.IsNullOrEmpty(shippingStatusIdentifier) ? null : new Dictionary<string, object>() { { "shippingStatus", shippingStatusIdentifier } };

            return (await _apiRepository.GetManyAsync<TOrderModel>(queryModel, headers, contextModel, $"ofcustomerid-{customerId}", extraParameters: statusFilter)).ToList();
        }

        public async Task<List<TOrderModel>> GetOrders(string customerIdentifier, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null, string shippingStatusIdentifier = null)
        {
            var statusFilter = String.IsNullOrEmpty(shippingStatusIdentifier) ? null : new Dictionary<string, object>() { { "shippingStatus", shippingStatusIdentifier } };

            return (await _apiRepository.GetManyAsync<TOrderModel>(queryModel, headers, contextModel, $"ofcustomer-{customerIdentifier}", extraParameters:statusFilter)).ToList();
        }

        public async Task<bool> DeleteCustomer(string customerIdentifier) => await _apiRepository.DeleteObjectAsync<TCustomerModel>(customerIdentifier);

        public async Task<bool> DeleteCustomer(int customerId) => await _apiRepository.DeleteObjectAsync<TCustomerModel>(customerId);
    }
}
