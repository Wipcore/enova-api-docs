using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces.Cart;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    public interface ICartService
    {

        /// <summary>
        /// Get an existing cart, mapped to a model.
        /// </summary>
        ICalculatedCartModel GetCart(string identifier = null, int id = 0);

        /// <summary>
        /// Maps given model to a cart in Enova and returns a model with prices specified.  
        /// </summary>
        ICalculatedCartModel CalculateCart(ICartModel model);

        /// <summary>
        /// Map order rows between cart and model.
        /// </summary>
        void MapCart(Context context, EnovaCart enovaCart, ICalculatedCartModel cartModel);

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
