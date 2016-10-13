using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.eNova.Api.WebApi.Helpers;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Customer
{
    public class CustomerOrdersMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Orders" };
        public Type Type => typeof(EnovaCustomer);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            var orders = new List<object>();
            var customer = (EnovaCustomer) obj;

            foreach (var order in customer.Orders.OfType<EnovaOrder>().OrderByDescending(x => x.CreatedAt))
            {
                var miniOrder = new 
                {
                    OrderId = order.ID,
                    OrderIdentifier = order.Identifier,
                    OrderDate = order.CreatedAt,
                    NumberOfItems = order.GetOrderItems<EnovaProductOrderItem>().Sum(i => i.OrderedQuantity),
                    ShippingStatus = order.ShippingStatus != null ? order.ShippingStatus.Name : String.Empty,
                    TotalPriceInclTax = PriceHelper.TotalPriceInclTax(order)
                };
                orders.Add(miniOrder);
            }

            return orders;
        }

        public void MapToEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
