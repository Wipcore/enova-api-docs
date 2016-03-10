using System.Collections.Generic;

namespace Wipcore.Enova.Api.Models.Interfaces
{
    public interface ICartModel
    {
        IEnumerable<IRowModel> Rows { get; set; }

        string Name { get; set; }

        string Identifier { get; set; }

        string Customer { get; set; }

        bool Persist { get; set; }

        decimal TotalPriceInclTax { get; set; }

        decimal TotalPriceExclTax { get; set; }

        string Status { get; set; }

        IDictionary<string, object> AdditionalValues { get; set; }
    }
    
    
}
