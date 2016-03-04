using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Enova.Api.Models;

namespace Wipcore.Enova.Api.NetClient
{
    public class CartClient
    {
        HttpClientWrapper _clientWrapper;

        public CartClient(HttpClientSettings clientSettings)
        {
            _clientWrapper = new HttpClientWrapper(clientSettings);
        }

        public async Task<ResponseModel> GetCart(string identifier, ContextModel context = null)
        {
            var response = await _clientWrapper.Get("api/carts/" + identifier + ContextHelper.GetContextParameters(context));
            var json = await response.Content.ReadAsStringAsync();

            var model = new ResponseModel();
            model.StatusCode = response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                // TODO: change to CartModel
                //model.Object = JsonConvert.DeserializeObject<CartModel>(json);
                model.Object = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            }

            return model;
        }

        public async Task<ResponseModel> SaveCart(CartModel cart, ContextModel context = null)
        {
            // TODO: better errorhandling
            var response = await _clientWrapper.Put("api/carts/" + ContextHelper.GetContextParameters(context), JsonConvert.SerializeObject(cart));
            var json = await response.Content.ReadAsStringAsync();

            var model = new ResponseModel();
            model.StatusCode = response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                model.Object = JsonConvert.DeserializeObject<CartModel>(json);
            }

            return model;
        }

        public async Task<ResponseModel> RecalculatePrices(CartModel cart, ContextModel context = null)
        {
            // TODO: better errorhandling
            string jsonRequest = JsonConvert.SerializeObject(cart);
            var response = await _clientWrapper.Post("api/carts/" + ContextHelper.GetContextParameters(context), jsonRequest);
            var json = await response.Content.ReadAsStringAsync();

            var model = new ResponseModel();
            model.StatusCode = response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                model.Object = JsonConvert.DeserializeObject<CartModel>(json);
            }

            return model;
        }
    }
}
