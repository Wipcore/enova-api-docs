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

namespace Wipcore.eNova.Api.NETClient
{
    public class CustomerRepository<TCustomerModel, TCartModel, TOrderModel> where TCartModel : CartModel where TOrderModel : OrderModel where TCustomerModel : CustomerModel
    {
        private readonly IApiRepository _apiRepository;
        private readonly Func<IApiClient> _apiClient;

        public CustomerRepository(IApiRepository apiRepository, Func<IApiClient> apiClient)
        {
            _apiRepository = apiRepository;
            _apiClient = apiClient;
        }

        public ILoginResponseModel LoginCustomer(string alias, string password) => _apiClient.Invoke().LoginCustomer(alias, password);

        public ILoginResponseModel LoginCustomerAsAdmin(string customerAlias, string adminAlias, string adminPassword) => _apiClient.Invoke().LoginCustomerAsAdmin(customerAlias, adminAlias, adminPassword);


        public TCustomerModel GetSavedCustomer(int id, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return _apiRepository.GetObject<TCustomerModel>(id, queryModel, contextModel);
        }

        public TCustomerModel GetSavedCustomer(string identifier, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return _apiRepository.GetObject<TCustomerModel>(identifier, queryModel, contextModel);
        }

        public TCustomerModel CreateCustomer(TCustomerModel customer, ContextModel contextModel = null)
        {
            return (TCustomerModel) _apiRepository.SaveObject<TCustomerModel>(JObject.FromObject(customer), contextModel: contextModel, verifyIdentifierNotTaken: true);
        }

        public TCustomerModel UpdateCustomer(TCustomerModel customer, ContextModel contextModel = null)
        {
            return (TCustomerModel)_apiRepository.SaveObject<TCustomerModel>(JObject.FromObject(customer), contextModel: contextModel, verifyIdentifierNotTaken: false);
        }

        public List<TCartModel> GetCarts(int customerId, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return _apiRepository.GetMany<TCartModel>(queryModel, headers, contextModel, $"ofcustomerid-{customerId}").ToList();
        }

        public List<TCartModel> GetCarts(string customerIdentifier, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return _apiRepository.GetMany<TCartModel>(queryModel, headers, contextModel, $"ofcustomer-{customerIdentifier}").ToList();
        }

        public List<TOrderModel> GetOrders(int customerId, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null, string shippingStatusIdentifier = null)
        {
            var statusFilter = String.IsNullOrEmpty(shippingStatusIdentifier) ? null : new Dictionary<string, object>() { { "shippingStatus", shippingStatusIdentifier } };

            return _apiRepository.GetMany<TOrderModel>(queryModel, headers, contextModel, $"ofcustomerid-{customerId}", extraParameters: statusFilter).ToList();
        }

        public List<TOrderModel> GetOrders(string customerIdentifier, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null, string shippingStatusIdentifier = null)
        {
            var statusFilter = String.IsNullOrEmpty(shippingStatusIdentifier) ? null : new Dictionary<string, object>() { { "shippingStatus", shippingStatusIdentifier } };

            return _apiRepository.GetMany<TOrderModel>(queryModel, headers, contextModel, $"ofcustomer-{customerIdentifier}", extraParameters:statusFilter).ToList();
        }

        public bool DeleteCustomer(string customerIdentifier) => _apiRepository.DeleteObject<TCustomerModel>(customerIdentifier);

        public bool DeleteCustomer(int customerId) => _apiRepository.DeleteObject<TCustomerModel>(customerId);
    }
}
