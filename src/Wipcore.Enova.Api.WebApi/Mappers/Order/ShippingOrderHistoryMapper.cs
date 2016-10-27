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
    public class ShippingOrderHistoryMapper : IPropertyMapper
    {
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        public List<string> Names => new List<string>() {"ShippingHistory"};

        public Type Type => typeof (EnovaOrder);

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var order = (EnovaOrder) obj;
            var context = order.GetContext();
            var history = from h in context.SearchInDatabase<EnovaOrderMovementLog>("OrderID = " + obj.ID) 
                          let fro = context.FindObject<EnovaShippingStatus>(h.FromStatusID)
                          let to = context.FindObject<EnovaShippingStatus>(h.ToStatusID)
                          select new
                            {
                                ID = h.ID,
                                Identifier = h.Identifier,
                                CreatedAt = h.CreatedAt,
                                fromStatusIdentifier = fro?.Identifier,
                                fromStatusName = fro?.Name,
                                toStatusIdentifier = to?.Identifier,
                                toStatusName = to?.Name,
                            };

            return history;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }

    }
}
