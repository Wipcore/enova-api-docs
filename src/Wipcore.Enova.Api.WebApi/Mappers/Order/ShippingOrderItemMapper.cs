using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Order
{
    public class ShippingOrderItemMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public bool InheritMapper => true;
        public bool FlattenMapping => false;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        public List<string> Names => new List<string>() { "ShippingOrderItem" };

        public Type Type => typeof(EnovaOrder);

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var order = (EnovaOrder)obj;
            var orderItems = order.GetOrderItems<EnovaShippingTypeOrderItem>().Select(x => new Dictionary<string, object>()
            {
                {"ID",  x.ID},
                {"ShippingID",  x.ShippingType?.ID ?? 0},
                {"Identifier", x.Identifier},
                {"ShippingIdentifier", x.ShippingType?.Identifier ?? String.Empty},
                {"PriceExclTax", x.GetPrice(false)},
                {"PriceInclTax", x.GetPrice(true)}
            }.MapLanguageProperty("Name", mappingLanguages, x.GetName)).FirstOrDefault();
            return orderItems;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
