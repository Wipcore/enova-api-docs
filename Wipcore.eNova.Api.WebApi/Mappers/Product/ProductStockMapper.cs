using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Product
{
    public class ProductStockMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() {"Stock", "Reserved"};
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

            return propertyName.Equals("Stock", StringComparison.InvariantCultureIgnoreCase) ? 
                compartments.Cast<EnovaWarehouseCompartment>().Sum(x => x.Stock) : 
                compartments.Cast<EnovaWarehouseCompartment>().Sum(x => x.Reserved);
        }
    }
}
