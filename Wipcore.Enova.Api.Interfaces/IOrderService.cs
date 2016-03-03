using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IOrderService
    {
        ICartModel CreateOrder(ICartModel cartModel);
    }
}