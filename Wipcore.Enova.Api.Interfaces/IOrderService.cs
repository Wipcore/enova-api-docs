using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IOrderService
    {
        ICartModel CreateOrder(ICartModel cartModel);

        BaseObjectList GetOrdersByCustomer(string customerIdentifier, string shippingStatus = null);
    }
}