using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Interfaces;

namespace Wipcore.eNova.Api.WebApi.Models
{
    public class CartModel : ICartModel //TODO handle promos, discounted prices and such stuff. Also shipping 
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
        public IDictionary<string, object> AdditionalValues { get; set; }
    }

    public class RowModel : IRowModel
    {
        public string Product { get; set; }
        public double Quantity { get; set; }
        public decimal PriceInclTax { get; set; }
        public decimal PriceExclTax { get; set; }
        public IDictionary<string, object> AdditionalValues { get; set; }
    }
}
