using System.Collections.Generic;
using Wipcore.Enova.Api.Models.Cart;

namespace Wipcore.Enova.Api.Models.Interfaces.Cart
{
    /// <summary>
    /// Model for a cart / order row.
    /// </summary>
    public interface ICartRowModel
    {
        /// <summary>
        /// The type of row (product, payment, shipping etc)
        /// </summary>
        RowType Type { get; set; }

        /// <summary>
        /// Identifier of the product, payment, shippingtype etc (depending on which type of row it is).
        /// </summary>
        string Identifier { get; set; }

        /// <summary>
        /// Password for unlocking a promo row.
        /// </summary>
        string Password { get; set; }

        double Quantity { get; set; }

        /// <summary>
        /// Dictionary of property-values.
        /// </summary>
        IDictionary<string, object> AdditionalValues { get; set; }
    }
}
