using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.Interfaces
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

        IDictionary<string, object> AdditionalValues { get; set; }
    }
    
    
}
