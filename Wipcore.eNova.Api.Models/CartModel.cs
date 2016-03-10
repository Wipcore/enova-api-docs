using System.Collections.Generic;
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

        public bool Persist { get; set; }

        public decimal TotalPriceInclTax { get; set; }

        public decimal TotalPriceExclTax { get; set; }
        public string Status { get; set; }
        public IDictionary<string, object> AdditionalValues { get; set; }
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
    }
}
