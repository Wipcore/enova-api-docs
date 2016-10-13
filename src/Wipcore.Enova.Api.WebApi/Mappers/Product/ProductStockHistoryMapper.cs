using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Product
{
    public class ProductStockHistoryMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "StockHistory" };
        public Type CmoType => typeof(CmoEnovaBaseProduct);
        public Type Type => typeof(EnovaBaseProduct);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        public void MapToEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
        }

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
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
