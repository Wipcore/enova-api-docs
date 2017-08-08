using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces.Cart;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.Abstractions.Internal
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
        /// <param name="customerIdentifier">Identifier of the customer. Not used if customerId is set.</param>
        /// <param name="customerId"></param>
        /// <param name="shippingStatus">Filter by shippingstatus identifier.</param>
        BaseObjectList GetOrdersByCustomer(int customerId = 0, string customerIdentifier = null, string shippingStatus = null);

        ///<summary>
        /// Get an existing order, mapped to a model.
        /// </summary>
        ICalculatedCartModel GetOrder(string identifier = null, int id = 0);

        /// <summary>
        /// Get a list of identifiers|names of the valid new shipping statuses for the given order.
        /// </summary>
        IDictionary<string, string> GetValidShippingStatuses(EnovaOrder order, bool includeCurrentStatus, bool allValidIfNoStatus);
    }
}