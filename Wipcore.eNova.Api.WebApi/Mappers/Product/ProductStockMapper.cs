using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Product
{
    public class ProductStockMapper : IPropertyMapper, ICmoProperty
    {
        public List<string> Names => new List<string>() {"TotalStock", "TotalReserved" };
        public Type CmoType => typeof (CmoEnovaBaseProduct);
        public Type Type => typeof (EnovaBaseProduct);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFrom;
        
        public object MapToEnovaProperty(BaseObject obj, string propertyName)
        {
            throw new NotImplementedException();
        }

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            var product = (EnovaBaseProduct) obj;
            var compartments = product.GetWarehouseCompartments(typeof (EnovaWarehouseCompartment));

            return propertyName.Equals("TotalStock", StringComparison.InvariantCultureIgnoreCase) ? 
                compartments.Cast<EnovaWarehouseCompartment>().Sum(x => x.Stock) : 
                compartments.Cast<EnovaWarehouseCompartment>().Sum(x => x.Reserved);
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
