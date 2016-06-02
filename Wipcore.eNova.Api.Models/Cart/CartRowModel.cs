using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Api.Models.Interfaces.Cart;

namespace Wipcore.Enova.Api.Models.Cart
{
    /// <summary>
    /// Model for a cart / order row.
    /// </summary>
    public class CartRowModel : ICartRowModel
    {
        /// <summary>
        /// The type of row (product, payment, shipping etc)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RowType Type { get; set; }

        /// <summary>
        /// Identifier of the product, payment, shippingtype etc (depending on which type of row it is).
        /// </summary>
        public string Identifier { get; set; }
        
        /// <summary>
        /// Password for unlocking a promo row.
        /// </summary>
        public string Password { get; set; }
        public double Quantity { get; set; }

        /// <summary>
        /// Dictionary of property-values.
        /// </summary>
        public IDictionary<string, object> AdditionalValues { get; set; }

        public override string ToString()
        {
            var addVal = AdditionalValues == null ? String.Empty : $"[{String.Join(", ", AdditionalValues.Select(x => $"{x.Key}:{x.Value}"))}]";
            var pass = Password == null ? null : "****";

            return $"Type: {Type}, Identifier: {Identifier}, Pass: {pass}, Quantity: {Quantity}, AdditionalValues: {addVal}";
        }
    }
}
