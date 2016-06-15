using System.Collections.Generic;

namespace Wipcore.Enova.Api.Models.Interfaces.Cart
{
    /// <summary>
    /// Model for posting a cart / order.
    /// </summary>
    public interface ICartModel
    {
        IEnumerable<ICartRowModel> Rows { get; set; }

        string Identifier { get; set; }

        /// <summary>
        /// Identifier of customer owning the cart / order.
        /// </summary>
        string Customer { get; set; }

        /// <summary>
        /// Set true to save the cart in the database.
        /// </summary>
        bool Persist { get; set; }

        /// <summary>
        /// Shipping status, applies to orders.
        /// </summary>
        string Status { get; set; }

        /// <summary>
        /// Dictionary of property-values.
        /// </summary>
        IDictionary<string, object> AdditionalValues { get; set; }
    }
}
