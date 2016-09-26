using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Api.Models.Interfaces.Cart;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface ICartService
    {
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
        BaseObjectList GetCartsByCustomer(string customerIdentifier);
    }
}
