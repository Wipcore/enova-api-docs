using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Order
{
    public class ProductOrderItemMapper : IPropertyMapper
    {
        private readonly IConfigService _configService;

        public ProductOrderItemMapper(IConfigService configService)
        {
            _configService = configService;
        }

        public bool PostSaveSet => false;
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;
        public bool FlattenMapping => false;
        public List<string> Names => new List<string>() { "ProductOrderItems" };

        public Type Type => typeof(EnovaOrder);

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var order = (EnovaOrder)obj;
            var context = obj.GetContext();

            var orderItems = from x in order.GetOrderItems<EnovaProductOrderItem>()
                             let priceExclTax = x.GetPrice(false)
                             let priceInclTax = x.GetPrice(true)
                             select new Dictionary<string, object>() {
                                {"ID", x.ID},
                                {"ProductID", x.Product?.ID ?? 0},
                                {"Identifier", x.Identifier},
                                {"ProductIdentifier", x.ProductIdentifier},
                                {"PriceExclTax", priceExclTax},
                                {"PriceInclTax", priceInclTax},
                                {"PriceExclTaxString", context.AmountToString(priceExclTax, context.CurrentCurrency, _configService.DecimalsInAmountString())},
                                {"PriceInclTaxString", context.AmountToString(priceInclTax, context.CurrentCurrency, _configService.DecimalsInAmountString())},
                                {"TotalPriceExclTaxString", context.AmountToString(priceExclTax * (decimal)x.OrderedQuantity, context.CurrentCurrency, _configService.DecimalsInAmountString())},
                                {"TotalPriceInclTaxString", context.AmountToString(priceInclTax * (decimal)x.OrderedQuantity, context.CurrentCurrency, _configService.DecimalsInAmountString())},
                                {"OrderedQuantity", x.OrderedQuantity},
                                {"Comment", x.Comment }
                            }.MapLanguageProperty("Name", mappingLanguages, x.GetName);

            return orderItems;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
