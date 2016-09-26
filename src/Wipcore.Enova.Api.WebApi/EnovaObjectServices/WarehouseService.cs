using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.EnovaObjectServices
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IContextService _contextService;

        public WarehouseService(IContextService contextService)
        {
            _contextService = contextService;
        }

        /// <summary>
        /// Get warehose compartments for the given product. 
        /// </summary>
        /// <param name="productIdentifier"></param>
        /// <param name="warehouseIdentifier">Filter compartments by specific warehose.</param>
        /// <returns></returns>
        public BaseObjectList GetWarehouseCompartments(string productIdentifier, string warehouseIdentifier = null)
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
