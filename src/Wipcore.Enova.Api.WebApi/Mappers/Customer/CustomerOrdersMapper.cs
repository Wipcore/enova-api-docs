using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.eNova.Api.WebApi.Helpers;
using Wipcore.Enova.Api.Abstractions.Interfaces;
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
        public bool PostSaveSet => false;
        public bool FlattenMapping => false;
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var orders = new List<object>();
            var customer = (EnovaCustomer) obj;

            foreach (var order in customer.Orders.OfType<EnovaOrder>().OrderByDescending(x => x.CreatedAt))
            {
                var miniOrder = new Dictionary<string, object>()
                {
                    {"OrderId", order.ID},
                    {"OrderIdentifier", order.Identifier},
                    {"NumberOfItems", order.GetOrderItems<EnovaProductOrderItem>().Sum(i => i.OrderedQuantity)},
                    {"TotalPriceInclTax", PriceHelper.TotalPriceInclTax(order)},
                    {"OrderDate", order.CreatedAt},
                }.MapLanguageProperty("ShippingStatus", mappingLanguages, language => order.ShippingStatus != null ? order.ShippingStatus.GetName(language) : String.Empty);

                orders.Add(miniOrder);
            }

            return orders;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
