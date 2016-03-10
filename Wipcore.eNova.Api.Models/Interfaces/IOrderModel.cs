using System.Collections.Generic;

namespace Wipcore.Enova.Api.Models.Interfaces
{
    public interface IOrderModel
    {
        IEnumerable<IOrderRowModel> Rows { get; set; }
        string Name { get; set; }
        string Identifier { get; set; }
        string Customer { get; set; }
        decimal TotalPriceInclTax { get; set; }
        decimal TotalPriceExclTax { get; set; }
        IDictionary<string, object> AdditionalValues { get; set; }
    }
}