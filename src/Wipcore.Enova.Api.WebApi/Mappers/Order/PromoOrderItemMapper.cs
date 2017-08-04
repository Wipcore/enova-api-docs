using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Order
{
    public class PromoOrderItemMapper : IPropertyMapper
    {
        private readonly IConfigService _configService;

        public PromoOrderItemMapper(IConfigService configService)
        {
            _configService = configService;
        }

        public bool PostSaveSet => false;
        public bool InheritMapper => true;
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        public List<string> Names => new List<string>() { "PromoOrderItems" };

        public Type Type => typeof(EnovaOrder);

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var order = (EnovaOrder)obj;
            var context = obj.GetContext();
            var orderItems = order.GetOrderItems<EnovaPromoOrderItem>().Select(x => new Dictionary<string, object>()
            {
                { "ID", x.ID},
                { "PromoID",  x.Promo?.ID ?? 0},
                { "Identifier", x.Identifier},
                { "PromoIdentifier", x.Promo?.Identifier ?? String.Empty},
                { "PriceExclTax", x.GetPrice(false)},
                { "PriceInclTax", x.GetPrice(true)},
                { "PriceExclTaxString", context.AmountToString(x.GetPrice(false), context.CurrentCurrency, _configService.DecimalsInAmountString())},
                { "PriceInclTaxString", context.AmountToString(x.GetPrice(true), context.CurrentCurrency, _configService.DecimalsInAmountString())},
                { "OrderedQuantity", x.OrderedQuantity}
            }.MapLanguageProperty("Name", mappingLanguages, x.GetName));

            return orderItems;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
