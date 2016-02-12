using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IRowModel
    {
        string Product { get; set; }

        double Quantity { get; set; }

        decimal PriceInclTax { get; set; }

        decimal PriceExclTax { get; set; }

        IDictionary<string, object> AdditionalValues { get; set; }
    }
}
