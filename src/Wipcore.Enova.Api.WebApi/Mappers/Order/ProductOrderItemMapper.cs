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
    public class ProductOrderItemMapper : IPropertyMapper
    {
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
            var orderItems = order.GetOrderItems<EnovaProductOrderItem>().Select(x => new
            {
                ID = x.ID,
                ProductID = x.Product?.ID ?? 0,
                Identifier = x.Identifier,
                ProductIdentifier = x.ProductIdentifier,
                PriceExclTax = x.GetPrice(false),
                PriceInclTax = x.GetPrice(true),
                OrderedQuantity = x.OrderedQuantity
            });
            return orderItems;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
