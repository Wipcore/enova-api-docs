using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.Mappers.Product
{
    /// <summary>
    /// Map stock, reserved and stock compartments information for a product.
    /// </summary>
    public class ProductStockMapper : IPropertyMapper, ICmoProperty
    {
        public List<string> Names => new List<string>() {"TotalStock", "TotalReserved", "Compartments" };
        public Type CmoType => typeof (CmoEnovaBaseProduct);
        public Type Type => typeof (EnovaBaseProduct);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapAll;
        
        public void MapToEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if(propertyName.Equals("TotalStock", StringComparison.InvariantCultureIgnoreCase) || propertyName.Equals("TotalReserved", StringComparison.InvariantCultureIgnoreCase))
                return;

            var context = obj.GetContext();
            dynamic compartments = value;
            foreach (var c in compartments)
            {
                var compartment = (Dictionary < string, object>)JsonConvert.DeserializeObject(c.ToString(), typeof (Dictionary<string, object>));

                var enovaCompartment = EnovaWarehouseCompartment.Find(context, compartment.GetOrDefault<int>("Id"));
                if(enovaCompartment.Stock == compartment.GetOrDefault<double>("Quantity"))
                    continue;

                var stockIn = enovaCompartment.Stock;
                enovaCompartment.Edit();
                enovaCompartment.Stock = compartment.GetOrDefault<double>("Quantity");

                var stockLog = new EnovaAdjustStockLog(context)
                {
                    UserID = context.GetAdministrator().ID,
                    ProductID = enovaCompartment.Product.ID,
                    ProductIdentifier = enovaCompartment.Product.Identifier,
                    CompartmentID = enovaCompartment.ID,
                    StockIn = stockIn,
                    StockOut = compartment.GetOrDefault<double>("Quantity"),
                    AdjustmentType = (int)EnovaAdjustStockLog.AdjustmentTypeEnum.ManualAdjustment,
                    Comment = compartments.Comment ?? "Adjusted from web (no reason given)"
                };

                stockLog.Save();
                enovaCompartment.Save();
            }
        }

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            var product = (EnovaBaseProduct) obj;
            var compartments = product.GetWarehouseCompartments(typeof (EnovaWarehouseCompartment)).Cast<EnovaWarehouseCompartment>();

            if (propertyName.Equals("TotalStock", StringComparison.InvariantCultureIgnoreCase))
                return compartments.Sum(x => x.Stock);
            if (propertyName.Equals("TotalReserved", StringComparison.InvariantCultureIgnoreCase))
                return compartments.Sum(x => x.Reserved);

            return compartments.Select(x => new
            {
                ID = x.ID,
                Quantity = x.Stock,
                ReservedQuantity = x.Reserved,
                WarehouseName = x.Warehouse?.Name,
                CompartmentName = x.Name
            });
        }

        public object GetProperty(CmoDbObject obj, CmoContext context, string propertyName, CmoLanguage language)
        {
            var product = (CmoEnovaBaseProduct)obj;
            var arrayList = new ArrayList();
            product.GetWarehouseCompartments(context, arrayList, typeof(CmoEnovaWarehouseCompartment), null);
            return propertyName.Equals("TotalStock", StringComparison.InvariantCultureIgnoreCase) ? 
                arrayList.Cast<CmoEnovaWarehouseCompartment>().Sum(x => x.Stock) : 
                arrayList.Cast<CmoEnovaWarehouseCompartment>().Sum(x => x.Reserved);
        }
    }
}
