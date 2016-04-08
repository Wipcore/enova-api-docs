using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.ServicesEnovaObjects
{
    public class WarehoseService : IWarehoseService
    {
        private readonly IContextService _contextService;

        public WarehoseService(IContextService contextService)
        {
            _contextService = contextService;
        }

        public BaseObjectList GetWarehoseCompartments(string productIdentifier, string warehouseIdentifier = null)
        {
            var context = _contextService.GetContext();
            var product = EnovaBaseProduct.Find(context, productIdentifier);

            EnovaWarehouse warehouse = null;
            if(!String.IsNullOrEmpty(warehouseIdentifier))
                warehouse = EnovaWarehouse.Find(context, warehouseIdentifier);

            return product.GetWarehouseCompartments(typeof (EnovaWarehouseCompartment), warehouse);
        }
    }
}
