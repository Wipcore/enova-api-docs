using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface ICartModel
    {
        IEnumerable<ICartRow> CartRows { get; set; }

        string Name { get; set; }

        string Identifier { get; set; }

        bool Persist { get; set; }

        decimal TotalPriceInclTax { get; set; }

        decimal TotalPriceExclTax { get; set; }

        IDictionary<string, object> AdditionalValues { get; set; }
    }
    
    public interface ICartRow
    {
        string Product { get; set; }

        double Quantity { get; set; }

        decimal PriceInclTax { get; set; }

        decimal PriceExclTax { get; set; }

        IDictionary<string, object> AdditionalValues { get; set; }
    }
}
