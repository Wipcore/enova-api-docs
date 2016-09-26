using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Models;
using Newtonsoft.Json;

namespace Wipcore.Enova.Api.NetClient
{
    public class CustomerClient
    {
        private readonly HttpClientWrapper _clientWrapper;

        public CustomerClient(HttpClientSettings clientSettings)
        {
            _clientWrapper = new HttpClientWrapper(clientSettings);
        }

        public ResponseModel GetCustomer(string identifier, ContextModel context = null)
        {
            var response = _clientWrapper.Get("api/customers/" + identifier + ContextHelper.GetContextParameters(context));
            var json = response.Content.ReadAsStringAsync().Result;

            var model = new ResponseModel {StatusCode = response.StatusCode};
            if (response.IsSuccessStatusCode)
            {
                model.Object = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            }

            return model;
        }
        
        public ResponseModel SaveCustomer(CustomerModel customerModel, ContextModel context = null)
        {
            string jsonRequest = JsonConvert.SerializeObject(customerModel);
            var response = _clientWrapper.Put("api/customers?" + ContextHelper.GetContextParameters(context), jsonRequest);
            var json = response.Content.ReadAsStringAsync().Result;

            var model = new ResponseModel {StatusCode = response.StatusCode};
            if (response.IsSuccessStatusCode)
            {
                model.Object = JsonConvert.DeserializeObject<CustomerModel>(json);
            }
            return model;
        }
        
    }
}
