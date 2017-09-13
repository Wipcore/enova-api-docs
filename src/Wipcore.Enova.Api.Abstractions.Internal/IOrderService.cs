using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    public interface IOrderService
    {
        /// <summary>
        /// Get orders owned by given customer.
        /// </summary>
        /// <param name="customerIdentifier">Identifier of the customer. Not used if customerId is set.</param>
        /// <param name="customerId"></param>
        /// <param name="shippingStatus">Filter by shippingstatus identifier.</param>
        BaseObjectList GetOrdersByCustomer(int customerId = 0, string customerIdentifier = null, string shippingStatus = null);
      

        /// <summary>
        /// Get a list of identifiers|names of the valid new shipping statuses for the given order.
        /// </summary>
        IDictionary<string, string> GetValidShippingStatuses(EnovaOrder order, bool includeCurrentStatus, bool allValidIfNoStatus);
    }
}