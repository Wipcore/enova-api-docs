using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;
using CartModel = Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart.CartModel;


namespace Wipcore.Enova.Api.NetClient.Cart
{
    public class CartRepositoryAsync<TCartModel, TOrderModel> where TCartModel : CartModel where TOrderModel : OrderModel
    {
        private readonly IApiRepositoryAsync _apiRepository;

        public CartRepositoryAsync(IApiRepositoryAsync apiRepository)
        {
            _apiRepository = apiRepository;
        }
        

        public async Task<TCartModel> Calculate(TCartModel cart, ContextModel contextModel = null)
        {
            return (TCartModel)await _apiRepository.SaveObjectAsync<TCartModel>(JObject.FromObject(cart), contextModel:contextModel, extraParameters: new Dictionary<string, object>() { { "calculateOnly", true } }, verifyIdentifierNotTaken:false);
        }

        public async Task<TOrderModel> TurnCartIntoOrder(TCartModel cart, ContextModel contextModel = null)
        {
            var orderId = (int)await _apiRepository.SaveObjectAsync<TCartModel>(JObject.FromObject(cart), contextModel: contextModel, action: "createorder", responseType:typeof(int), verifyIdentifierNotTaken:false);
            return await _apiRepository.GetObjectAsync<TOrderModel>(orderId, null, contextModel);
        }

        public async Task<TCartModel> GetSavedCart(int id, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return await _apiRepository.GetObjectAsync<TCartModel>(id, queryModel, contextModel);
        }

        public async Task<TCartModel> GetSavedCart(string identifier, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return await _apiRepository.GetObjectAsync<TCartModel>(identifier, queryModel, contextModel);
        }

        public async Task<TCartModel> CreateCart(TCartModel cart, ContextModel contextModel = null)
        {
            return (TCartModel)await _apiRepository.SaveObjectAsync<TCartModel>(JObject.FromObject(cart), contextModel: contextModel, verifyIdentifierNotTaken: true);
        }

        public async Task<TCartModel> UpdateCart(TCartModel cart, ContextModel contextModel = null)
        {
            return (TCartModel)await _apiRepository.SaveObjectAsync<TCartModel>(JObject.FromObject(cart), contextModel: contextModel, verifyIdentifierNotTaken: false);
        }

        public async Task<bool> DeleteCart(string cartIdentifier) => await _apiRepository.DeleteObjectAsync<TCartModel>(cartIdentifier);

        public async Task<bool> DeleteCart(int cartId) => await _apiRepository.DeleteObjectAsync<TCartModel>(cartId);
    }
}
