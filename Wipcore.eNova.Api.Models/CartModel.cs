using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.Models
{
    public class CartModel : ICartModel
    {
        public CartModel(IEnumerable<RowModel> rows)
        {
            Rows = rows;
        }

        public IEnumerable<IRowModel> Rows { get; set; }
        public string Name { get; set; }

        public string Identifier { get; set; }
        public string Customer { get; set; }

        /// <summary>
        /// Set true to save the cart in the database.
        /// </summary>
        public bool Persist { get; set; }

        public decimal TotalPriceInclTax { get; set; }

        public decimal TotalPriceExclTax { get; set; }
        public string Status { get; set; }
        public IDictionary<string, object> AdditionalValues { get; set; }

        public override string ToString()
        {
            var rows = Rows == null ? String.Empty : $"[{String.Join("| ", Rows.Select(x => x.ToString()))}]";
            var addVal = AdditionalValues == null ? String.Empty : $"[{String.Join(", ", AdditionalValues.Select(x => $"{x.Key}:{x.Value}"))}]";

            return $"CartModel: (Name: {Name}, Identifier: {Identifier}, Customer: {Customer}, Persist: {Persist}, TotalPriceExcl: {TotalPriceExclTax}, " +
                   $"TotalPriceInclTax: {TotalPriceInclTax}, Status: {Status}, Rows: {rows}, AdditionalValues: {addVal})";
        }
    }

    public class RowModel : IRowModel
    {
        public string Type { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public double Quantity { get; set; }
        public decimal PriceInclTax { get; set; }
        public decimal PriceExclTax { get; set; }
        public IDictionary<string, object> AdditionalValues { get; set; }

        public override string ToString()
        {
            var addVal = AdditionalValues == null ? String.Empty : $"[{String.Join(", ", AdditionalValues.Select(x => $"{x.Key}:{x.Value}"))}]";
            var pass = Password == null ? null : "****";

            return $"Type: {Type}, Identifier: {Identifier}, Name: {Name}, Pass: {pass}, Quantity: {Quantity}, " +
                   $"PriceInclTax: {PriceInclTax}, PriceExclTax: {PriceExclTax}, AdditionalValues: {addVal}";
        }
    }
}
