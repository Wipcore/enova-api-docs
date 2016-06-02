using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.Models.Cart
{
    /// <summary>
    /// Type of cart / order row.
    /// </summary>
    public enum RowType
    {
        Product,
        Payment,
        Promo,
        Shipping
    }
}
 