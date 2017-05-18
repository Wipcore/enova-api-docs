using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Order
{
    public class ShippingOrderHistoryMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public bool InheritMapper => true;
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        public List<string> Names => new List<string>() {"ShippingHistory"};

        public Type Type => typeof (EnovaOrder);

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var order = (EnovaOrder) obj;
            var context = order.GetContext();
            var history = from h in context.SearchInDatabase<EnovaOrderMovementLog>("OrderID = " + obj.ID) 
                          let fro = context.FindObject<EnovaShippingStatus>(h.FromStatusID)
                          let to = context.FindObject<EnovaShippingStatus>(h.ToStatusID)
                          select new Dictionary<string, object>()
                            {
                               {"ID", h.ID},
                               {"Identifier", h.Identifier},
                               {"CreatedAt", h.CreatedAt},
                               {"fromStatusIdentifier", fro?.Identifier ?? String.Empty},
                               {"toStatusIdentifier", to?.Identifier ?? String.Empty},
                            }.MapLanguageProperty("fromStatusName", mappingLanguages, language => fro?.GetName(language) ?? String.Empty)
                             .MapLanguageProperty("toStatusName", mappingLanguages, language => to?.GetName(language) ?? String.Empty); ;

            return history;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }

    }
}
