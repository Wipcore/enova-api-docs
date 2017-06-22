using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Product
{
    public class ProductStockHistoryMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public List<string> Names => new List<string>() { "StockHistory" };
        public Type CmoType => typeof(CmoEnovaBaseProduct);
        public Type Type => typeof(EnovaBaseProduct);
        public bool InheritMapper => true;
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
        }

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var context = obj.GetContext();
            var searchExpression = String.Format("ProductID='{0}'", obj.ID);
            var stocklog = context.SearchInDatabase<EnovaAdjustStockLog>(searchExpression).OrderByDescending(log => log.CreatedAt).Take(5); //TODO config this value? or make client go to product/history for more details?

            return stocklog.Select(x => new
            {
                Timestamp = x.CreatedAt,
                Comment = x.Comment,
                Stock = x.StockOut,
                User = x.UserID.ToString(),
                CompartmentName = context.FindObject<EnovaWarehouseCompartment>(x.CompartmentID)?.Name ?? String.Empty
            });
        }
    }
}
