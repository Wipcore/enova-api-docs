using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface ICartService
    {
        ICartModel CalculateCart(ICartModel currentCart, string customerIdentifier);
    }
}
