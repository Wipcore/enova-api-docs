using System.Collections.Generic;
using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.Models
{
    public class OrderModel : IOrderModel
    {
        public OrderModel(IEnumerable<OrderRowModel> rows)
        {
            Rows = rows;
        }

        public IEnumerable<IOrderRowModel> Rows { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Customer { get; set; }
        public decimal TotalPriceInclTax { get; set; }
        public decimal TotalPriceExclTax { get; set; }
        public IDictionary<string, object> AdditionalValues { get; set; }
    }

    public class OrderRowModel : IOrderRowModel
    {
        public string Type { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public decimal PriceInclTax { get; set; }
        public decimal PriceExclTax { get; set; }
        public IDictionary<string, object> AdditionalValues { get; set; }
    }
}