using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Enova.Api.Abstractions.Interfaces.Cart;

namespace Wipcore.Enova.Api.Abstractions.Models
{
    /// <summary>
    /// Model for posting a cart / order.
    /// </summary>
    public class CartModel : ICartModel
    {
        public CartModel(IEnumerable<CartRowModel> rows)//binds to concerete type for http model binding
        {
            Rows = rows;
        }

        public IEnumerable<ICartRowModel> Rows { get; set; }
        public string Identifier { get; set; }

        /// <summary>
        /// Identifier of customer owning the cart / order.
        /// </summary>
        public string Customer { get; set; }

        /// <summary>
        /// Set true to save the cart in the database.
        /// </summary>
        public bool Persist { get; set; }
        
        /// <summary>
        /// Shipping status, applies to orders.
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Dictionary of property-values.
        /// </summary>
        public IDictionary<string, object> AdditionalValues { get; set; }

        public override string ToString()
        {
            var rows = Rows == null ? String.Empty : $"[{String.Join("| ", Rows.Select(x => x.ToString()))}]";
            var addVal = AdditionalValues == null ? String.Empty : $"[{String.Join(", ", AdditionalValues.Select(x => $"{x.Key}:{x.Value}"))}]";

            return $"CartModel: (Identifier: {Identifier}, Customer: {Customer}, Persist: {Persist}, Status: {Status}, Rows: {rows}, AdditionalValues: {addVal})";
        }
    }
}
