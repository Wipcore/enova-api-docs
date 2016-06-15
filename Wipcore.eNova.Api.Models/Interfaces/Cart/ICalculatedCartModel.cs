using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.Models.Interfaces.Cart
{
    /// <summary>
    /// Model for a cart/order that has had it's prices calculated.
    /// </summary>
    public interface ICalculatedCartModel : ICartModel
    {
        decimal TotalPriceInclTax { get; set; }

        decimal TotalPriceExclTax { get; set; }
    }
}
