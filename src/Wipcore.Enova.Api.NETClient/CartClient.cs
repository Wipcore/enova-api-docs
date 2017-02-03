using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Enova.Api.Abstractions.Models;

namespace Wipcore.Enova.Api.NetClient
{
    public class CartClient
    {
        private readonly HttpClientWrapper _clientWrapper;

        public CartClient(HttpClientSettings clientSettings)
        {
            _clientWrapper = new HttpClientWrapper(clientSettings);
        }

        public ResponseModel GetCart(string identifier, ContextModel context = null)
        {
            var response = _clientWrapper.Get("api/carts/" + identifier + ContextHelper.GetContextParameters(context));
            var json = response.Content.ReadAsStringAsync().Result;

            var model = new ResponseModel {StatusCode = response.StatusCode};
            if (response.IsSuccessStatusCode)
            {
                // TODO: change to CartModel
                //model.Object = JsonConvert.DeserializeObject<CartModel>(json);
                model.Object = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            }

            return model;
        }

        public ResponseModel SaveCart(CartModel cart, ContextModel context = null)
        {
            // TODO: better errorhandling
            var response = _clientWrapper.Put("api/carts?" + ContextHelper.GetContextParameters(context), JsonConvert.SerializeObject(cart));
            var json = response.Content.ReadAsStringAsync().Result;

            var model = new ResponseModel {StatusCode = response.StatusCode};
            if (response.IsSuccessStatusCode)
            {
                model.Object = JsonConvert.DeserializeObject<CartModel>(json);
            }

            return model;
        }

        public ResponseModel RecalculatePrices(CartModel cart, ContextModel context = null)
        {
            // TODO: better errorhandling
            var jsonRequest = JsonConvert.SerializeObject(cart);
            var response = _clientWrapper.Post("api/carts?" + ContextHelper.GetContextParameters(context), jsonRequest);
            var json = response.Content.ReadAsStringAsync().Result;

            var model = new ResponseModel {StatusCode = response.StatusCode};
            if (response.IsSuccessStatusCode)
            {
                model.Object = JsonConvert.DeserializeObject<CartModel>(json);
            }

            return model;
        }
    }
}
