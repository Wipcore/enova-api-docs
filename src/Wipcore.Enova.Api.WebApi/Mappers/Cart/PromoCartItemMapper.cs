using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
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

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var cart = (EnovaCart)obj;

            var cartItems = cart.GetCartItems<EnovaPromoCartItem>().Select(x => new Dictionary<string, object>()
                {
                    {"ID", x.ID },
                    {"PromoID", x.Promo?.ID ?? 0 },
                    {"Identifier", x.Identifier },
                    {"PromoIdentifier", x.Promo?.Identifier ?? String.Empty },
                    {"PriceExclTax", x.GetPrice(false) },
                    {"PriceInclTax", x.GetPrice(true) },
                    {"Quantity", x.Quantity },
                }.MapLanguageProperty("Name", mappingLanguages, x.GetName));
            return cartItems;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            
        }
    }
}
