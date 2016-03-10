using System.Collections.Generic;

namespace Wipcore.Enova.Api.Models.Interfaces
{
    public interface IOrderRowModel
    {
        string Type { get; set; } //shipping, product, payment, promo
        string Identifier { get; set; }
        string Name { get; set; }
        double Quantity { get; set; }
        decimal PriceInclTax { get; set; }
        decimal PriceExclTax { get; set; }
        IDictionary<string, object> AdditionalValues { get; set; }
    }
}
