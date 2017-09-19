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
        private readonly IConfigService _configService;

        public PromoCartItemMapper(IConfigService configService)
        {
            _configService = configService;
        }

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
            var context = obj.GetContext();

            var cartItems = from x in cart.GetCartItems<EnovaPromoCartItem>()
                             let priceExclTax = x.GetPrice(false)
                             let priceInclTax = x.GetPrice(true)
                             select new Dictionary<string, object>() {
                                {"ID", x.ID},
                                {"PromoID",  x.Promo?.ID ?? 0},
                                {"Identifier", x.Identifier},
                                {"PromoIdentifier", x.Promo?.Identifier ?? String.Empty},
                                {"PriceExclTax", priceExclTax},
                                {"PriceInclTax", priceInclTax},
                                {"PriceExclTaxString", context.AmountToString(priceExclTax, context.CurrentCurrency, _configService.DecimalsInAmountString())},
                                {"PriceInclTaxString", context.AmountToString(priceInclTax, context.CurrentCurrency, _configService.DecimalsInAmountString())},
                                {"TotalPriceExclTaxString", context.AmountToString(priceExclTax * (decimal)x.Quantity, context.CurrentCurrency, _configService.DecimalsInAmountString())},
                                {"TotalPriceInclTaxString", context.AmountToString(priceInclTax * (decimal)x.Quantity, context.CurrentCurrency, _configService.DecimalsInAmountString())},
                                {"Quantity", x.Quantity},
                                {"Comment", x.Comment }
                            }.MapLanguageProperty("Name", mappingLanguages, x.GetName);
           
            return cartItems;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            
        }
    }
}
