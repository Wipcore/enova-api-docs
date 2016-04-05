using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface ICartService
    {
        ICartModel CalculateCart(ICartModel currentCart);

        void MapCart(Context context, EnovaCart enovaCart, ICartModel currentCart);

        BaseObjectList GetCartsByCustomer(string customerIdentifier);
    }
}
