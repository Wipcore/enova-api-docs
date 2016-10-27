using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Order
{
    public class ShippingOrderItemMapper : IPropertyMapper
    {
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        public List<string> Names => new List<string>() { "ShippingInfo" };

        public Type Type => typeof(EnovaOrder);

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var order = (EnovaOrder)obj;
            var orderItems = order.GetOrderItems<EnovaShippingTypeOrderItem>().Select(x => new
            {
                ID = x.ID,
                Identifier = x.Identifier,
                Name = x.Name,
                ShippingIdentifier = x.ShippingType?.Identifier,
                PriceExclTax = x.GetPrice(false),
                PriceInclTax = x.GetPrice(true)
            }).FirstOrDefault();
            return orderItems;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
