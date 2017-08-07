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

        public TCustomerModel CreateOrUpdateCustomer(TCustomerModel customer, ContextModel contextModel = null, bool verifyIdentifierNotTaken = true)
        {
            return (TCustomerModel) _apiRepository.SaveObject<TCustomerModel>(JObject.FromObject(customer), contextModel: contextModel, verifyIdentifierNotTaken: verifyIdentifierNotTaken);
        }

        public List<TCartModel> GetCarts(int customerId, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return _apiRepository.GetMany<TCartModel>(queryModel, headers, contextModel, $"ofcustomerid-{customerId}").ToList();
        }

        public List<TCartModel> GetCarts(string customerIdentifier, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return _apiRepository.GetMany<TCartModel>(queryModel, headers, contextModel, $"ofcustomer-{customerIdentifier}").ToList();
        }

        public List<TOrderModel> GetOrders(int customerId, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return null;
        }

        public List<TOrderModel> GetOrders(string customerIdentifier, ApiResponseHeadersModel headers = null, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return null;
        }
    }
}
