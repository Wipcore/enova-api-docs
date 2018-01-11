using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;

namespace Wipcore.Enova.Api.NetClient.Order
{
    public class OrderRepositoryAsync<TOrderModel> where TOrderModel : OrderModel
    {
        private readonly IApiRepositoryAsync _apiRepository;

        public OrderRepositoryAsync(IApiRepositoryAsync apiRepository)
        {
            _apiRepository = apiRepository;
        }

        public async Task<TOrderModel> GetSavedOrderAsync(int id, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return await _apiRepository.GetObjectAsync<TOrderModel>(id, queryModel, contextModel);
        }

        public async Task<TOrderModel> GetSavedOrderAsync(string identifier, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return await _apiRepository.GetObjectAsync<TOrderModel>(identifier, queryModel, contextModel);
        }

        public async Task<TOrderModel> CreateOrderAsync(TOrderModel order, ContextModel contextModel = null)
        {
            return (TOrderModel)await _apiRepository.SaveObjectAsync<TOrderModel>(JObject.FromObject(order), contextModel: contextModel, verifyIdentifierNotTaken: false);
        }

        public async Task<TOrderModel> UpdateOrderAsync(TOrderModel order, ContextModel contextModel = null)
        {
            return (TOrderModel) await _apiRepository.SaveObjectAsync<TOrderModel>(JObject.FromObject(order), contextModel: contextModel, verifyIdentifierNotTaken: true);
        }

        public async Task<bool> DeleteOrderAsync(string orderIdentifier) => await _apiRepository.DeleteObjectAsync<TOrderModel>(orderIdentifier);

        public async Task<bool> DeleteOrderAsync(int orderId) => await _apiRepository.DeleteObjectAsync<TOrderModel>(orderId);

    }
}
