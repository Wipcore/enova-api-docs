using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.NetClient;

namespace Wipcore.eNova.Api.NetClient
{
    public class CustomerClient
    {
        private readonly HttpClientWrapper _clientWrapper;

        public CustomerClient(HttpClientSettings clientSettings)
        {
            _clientWrapper = new HttpClientWrapper(clientSettings);
        }

        public async Task<ResponseModel> GetCustomer(string identifier, ContextModel context = null)
        {
            var response = await _clientWrapper.Get("api/customers/" + identifier + ContextHelper.GetContextParameters(context));
            var json = await response.Content.ReadAsStringAsync();

            var model = new ResponseModel {StatusCode = response.StatusCode};
            if (response.IsSuccessStatusCode)
            {
                model.Object = JsonConvert.DeserializeObject<IDictionary<string,object>>(json);
            }

            return model;
        }
        
        public async Task<ResponseModel> SaveCustomer(CustomerModel customerModel, ContextModel context = null)
        {
            var jsonRequest = JsonConvert.SerializeObject(customerModel);
            var response = await _clientWrapper.Put("api/customers?" + ContextHelper.GetContextParameters(context), jsonRequest);
            var json = await response.Content.ReadAsStringAsync();

            var model = new ResponseModel {StatusCode = response.StatusCode};
            if (response.IsSuccessStatusCode)
            {
                model.Object = JsonConvert.DeserializeObject<CustomerModel>(json);
            }
            return model;
        }

        public async Task<ResponseModel> GetCustomersCarts(string identifier, ContextModel context = null)
        {
            return await GetSubItems(identifier, "carts", context);
        }

        public async Task<ResponseModel> GetCustomersOrders(string identifier, ContextModel context = null)
        {
            return await GetSubItems(identifier, "carts", context);
        }

        public async Task<ResponseModel> GetCustomersAddresses(string identifier, ContextModel context = null)
        {
            return await GetSubItems(identifier, "addresses", context);
        }

        private async Task<ResponseModel> GetSubItems(string identifier, string subItem, ContextModel context = null)//TODO easy paging
        {
            var response = await _clientWrapper.Get("api/customers/" + identifier + "/" + subItem + ContextHelper.GetContextParameters(context));
            var json = await response.Content.ReadAsStringAsync();

            var model = new ResponseModel { StatusCode = response.StatusCode };
            if (response.IsSuccessStatusCode)
            {
                model.List = new ListModel() {Objects = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(json)};
            }

            return model;
        }
    }
}
