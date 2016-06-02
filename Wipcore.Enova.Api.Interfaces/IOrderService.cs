using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Api.Models.Interfaces.Cart;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IOrderService
    {
        /// <summary>
        /// Create a new order or update an order. Updates cannot change price/quantity of orderrows, that requires a new order.
        /// </summary>
        ICartModel SaveOrder(ICartModel cartModel);

        /// <summary>
        /// Get orders owned by given customer.
        /// </summary>
        /// <param name="customerIdentifier"></param>
        /// <param name="shippingStatus">Filter by shippingstatus identifier.</param>
        /// <returns></returns>
        BaseObjectList GetOrdersByCustomer(string customerIdentifier, string shippingStatus = null);
    }
}