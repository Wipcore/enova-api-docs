using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;
using CartModel = Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart.CartModel;


namespace Wipcore.Enova.Api.NetClient
{
    public class CartRepository<TCartModel, TOrderModel> where TCartModel : CartModel where TOrderModel : OrderModel
    {
        private readonly IApiRepository _apiRepository;

        public CartRepository(IApiRepository apiRepository)
        {
            _apiRepository = apiRepository;
        }
        

        public TCartModel Calculate(TCartModel cart, ContextModel contextModel = null)
        {
            return (TCartModel)_apiRepository.SaveObject<TCartModel>(JObject.FromObject(cart), contextModel:contextModel, extraParameters: new Dictionary<string, object>() { { "calculateOnly", true } }, verifyIdentifierNotTaken:false);
        }

        public TOrderModel TurnCartIntoOrder(TCartModel cart, ContextModel contextModel = null)
        {
            var orderId = (int)_apiRepository.SaveObject<TCartModel>(JObject.FromObject(cart), contextModel: contextModel, action: "createorder", responseType:typeof(int));
            return _apiRepository.GetObject<TOrderModel>(orderId, null, contextModel);
        }

        public TCartModel GetSavedCart(int id, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return _apiRepository.GetObject<TCartModel>(id, queryModel, contextModel);
        }

        public TCartModel GetSavedCart(string identifier, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return _apiRepository.GetObject<TCartModel>(identifier, queryModel, contextModel);
        }

        public TCartModel CreateOrUpdateCart(TCartModel cart, ContextModel contextModel = null, bool verifyIdentifierNotTaken = true)
        {
            return (TCartModel)_apiRepository.SaveObject<TCartModel>(JObject.FromObject(cart), contextModel: contextModel, verifyIdentifierNotTaken:verifyIdentifierNotTaken);
        }

        public bool DeleteCart(string cartIdentifier) => _apiRepository.DeleteObject<TCartModel>(cartIdentifier);

        public bool DeleteCart(int cartId) => _apiRepository.DeleteObject<TCartModel>(cartId);
    }
}
