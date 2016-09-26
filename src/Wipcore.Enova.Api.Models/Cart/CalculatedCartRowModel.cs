using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Api.Models.Interfaces.Cart;

namespace Wipcore.Enova.Api.Models.Cart
{
    /// <summary>
    /// Model for a cart/order row with it's prices calculated.
    /// </summary>
    public class CalculatedCartRowModel : CartRowModel, ICalculatedCartRowModel
    {
        public string Name { get; set; }
        public decimal PriceInclTax { get; set; }
        public decimal PriceExclTax { get; set; }

        public override string ToString()
        {
            var addVal = AdditionalValues == null ? String.Empty : $"[{String.Join(", ", AdditionalValues.Select(x => $"{x.Key}:{x.Value}"))}]";
            var pass = Password == null ? null : "****";

            return $"Type: {Type}, Identifier: {Identifier}, Name: {Name}, Pass: {pass}, Quantity: {Quantity}, " +
                   $"PriceInclTax: {PriceInclTax}, PriceExclTax: {PriceExclTax}, AdditionalValues: {addVal}";
        }
    }
}
