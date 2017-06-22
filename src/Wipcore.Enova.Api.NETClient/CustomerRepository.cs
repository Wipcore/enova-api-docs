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

        public CustomerRepository(IApiRepository apiRepository)
        {
            _apiRepository = apiRepository;
        }


        public TCustomerModel GetSavedCustomer(int id, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return _apiRepository.GetObject<TCustomerModel>(id, queryModel, contextModel);
        }

        public TCustomerModel GetSavedCustomer(string identifier, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return _apiRepository.GetObject<TCustomerModel>(identifier, queryModel, contextModel);
        }

        public TCustomerModel CreateOrUpdateCustomer(TCustomerModel customer, ContextModel contextModel = null)
        {
            return (TCustomerModel) _apiRepository.SaveObject<TCustomerModel>(JObject.FromObject(customer), contextModel: contextModel);
        }

        public List<TCartModel> GetCarts(int customerId, ref ApiResponseHeadersModel headers, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return null;
        }

        public List<TCartModel> GetCarts(string customerIdentifier, ref ApiResponseHeadersModel headers, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return null;
        }

        public List<TOrderModel> GetOrders(int customerId, ref ApiResponseHeadersModel headers, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return null;
        }

        public List<TOrderModel> GetOrders(string customerIdentifier, ref ApiResponseHeadersModel headers, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return null;
        }
    }
}
