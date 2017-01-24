using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.eNova.Api.WebApi.Helpers;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Cart
{
    public class PromoCartItemMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public List<string> Names => new List<string>() { "PromoCartItems" };

        public Type Type => typeof(EnovaCart);
        public bool FlattenMapping => false;

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var cart = (EnovaCart)obj;
            var cartItems = cart.GetCartItems<EnovaPromoCartItem>().Select(x => new
            {
                ID = x.ID,
                Identifier = x.Identifier,
                Name = x.Name,
                PromoIdentifier = x.Promo?.Identifier,
                PriceExclTax = x.GetPrice(false),
                PriceInclTax = x.GetPrice(true),
                Quantity = x.Quantity
            });
            return cartItems;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            
        }
    }
}
