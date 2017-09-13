using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    public interface ICartService
    {
        /// <summary>
        /// Get all carts belonging to given customer.
        /// </summary>
        BaseObjectList GetCartsByCustomer(string customerIdentifier = null, int customerId = 0);

        /// <summary>
        /// Creates an order from a mapping to cart by given values.
        /// </summary>
        int CreateOrderFromCart(ContextModel requestContext, Dictionary<string, object> values);

    }
}
